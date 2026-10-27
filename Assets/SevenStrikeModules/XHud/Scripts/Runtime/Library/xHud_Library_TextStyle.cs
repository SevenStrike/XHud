/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using TMPro;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;

    public enum xHud_TextType
    {
        Text = 0,
        TmpText = 1
    }

    [SerializeField]
    [System.Serializable]
    /// <summary>
    /// Hud字体信息
    /// </summary>
    public class XHud_LibraryArg_TextStyle
    {
        #region 字段
        //----------------------------------------Based----------------------------------------//
        [SerializeField]
        /// <summary>
        /// 样式名称
        /// </summary>
        public string name;
        [SerializeField]
        /// <summary>
        /// 文字模版解释
        /// </summary>
        public string description;
        [SerializeField]
        /// <summary>
        /// 文字组件类型
        /// </summary>
        public xHud_TextType type = xHud_TextType.Text;
        //----------------------------------------Text----------------------------------------//
        [SerializeField]
        /// <summary>
        /// 字体样式
        /// </summary>
        public FontStyle style = FontStyle.Normal;
        [SerializeField]
        /// <summary>
        /// 文字组件锚点
        /// </summary>
        public ContentAnchor content_anchor = ContentAnchor.中心;
        [SerializeField]
        /// <summary>
        /// 文字字体
        /// </summary>
        public Font font;
        [SerializeField]
        /// <summary>
        /// 文字字体路径
        /// </summary>
        public string font_asset_path;
        [SerializeField]
        [Range(0, 300)]
        /// <summary>
        /// 文字尺寸
        /// </summary>
        public float size = 13;
        [SerializeField]
        /// <summary>
        /// 文字行高
        /// </summary>
        public float line_height = 1;
        [SerializeField]
        /// <summary>
        /// 水平溢出
        /// </summary>
        public HorizontalWrapMode overflow_horizon = HorizontalWrapMode.Wrap;
        [SerializeField]
        /// <summary>
        /// 垂直溢出
        /// </summary>
        public VerticalWrapMode overflow_vertical = VerticalWrapMode.Truncate;
        [SerializeField]
        /// <summary>
        /// 富文本支持
        /// </summary>
        public bool rich_text = false;
        [SerializeField]
        /// <summary>
        /// 几何对齐
        /// </summary>
        public bool geometre_align = false;
        [SerializeField]
        /// <summary>
        /// 自动尺寸
        /// </summary>
        public bool best_fit = false;
        [SerializeField]
        /// <summary>
        /// 自动尺寸最小
        /// </summary>
        public int fit_min = 1;
        [SerializeField]
        /// <summary>
        /// 自动字体尺寸 最小 - 全局缩放倍增值
        /// </summary>
        public float fit_min_global_scaled;
        [SerializeField]
        [Range(0, 300)]
        /// <summary>
        /// 自动尺寸最大
        /// </summary>
        public int fit_max = 50;
        [SerializeField]
        /// <summary>
        /// 自动字体尺寸 最大 - 全局缩放倍增值
        /// </summary>
        public float fit_max_global_scaled;
        [SerializeField]
        /// <summary>
        /// 字体尺寸 - 全局缩放倍增值
        /// </summary>
        public float size_global_scaled;
        [SerializeField]
        /// <summary>
        /// 字体颜色
        /// </summary>
        public Color font_color = Color.white;
        //----------------------------------------Tmp Text----------------------------------------//
        [SerializeField]
        /// <summary>
        /// Tmp富文本支持
        /// </summary>
        public bool tmp_rich = false;
        [SerializeField]
        /// <summary>
        /// Tmp内容包裹 Old
        /// </summary>
        public TextWrappingModes tmp_contentwrap = TextWrappingModes.NoWrap;
        [SerializeField]
        /// <summary>
        /// Tmp溢出模式
        /// </summary>
        public TextOverflowModes tmp_overflow = TextOverflowModes.Overflow;
        [SerializeField]
        /// <summary>
        /// Tmp字体样式
        /// </summary>
        public FontStyles tmp_style = FontStyles.Normal;
        [SerializeField]
        /// <summary>
        /// Tmp锚点
        /// </summary>
        public TmpContentAnchor tmp_anchor = TmpContentAnchor.中心;
        [SerializeField]
        /// <summary>
        /// Tmp字体资源
        /// </summary>
        public TMP_FontAsset tmp_font;
        [SerializeField]
        /// <summary>
        /// Tmp字体路径
        /// </summary>
        public string tmp_fontasset_assetpath;
        [SerializeField]
        /// <summary>
        /// Tmp字体尺寸
        /// </summary>
        public float tmp_size = 13;
        [SerializeField]
        /// <summary>
        /// Tmp字体尺寸 - 全局缩放倍增值
        /// </summary>
        public float tmp_size_globalscaled;
        [SerializeField]
        /// <summary>
        /// Tmp字符间距
        /// </summary>
        public float tmp_space_character;
        [SerializeField]
        /// <summary>
        /// Tmp单词间距
        /// </summary>
        public float tmp_space_word;
        [SerializeField]
        /// <summary>
        /// Tmp行高
        /// </summary>
        public float tmp_space_lineheight;
        [SerializeField]
        /// <summary>
        /// Tmp段落间距
        /// </summary>
        public float tmp_space_paragraph;
        [SerializeField]
        [Range(0, 1)]
        /// <summary>
        /// Tmp均分对齐
        /// </summary>
        public float tmp_wrapping_ratios;
        [SerializeField]
        /// <summary>
        /// Tmp内容边距
        /// </summary>
        public Vector4 tmp_content_margin;
        [SerializeField]
        /// <summary>
        /// Tmp字体颜色
        /// </summary>
        public Color tmp_color = Color.white;
        [SerializeField]
        /// <summary>
        /// Tmp自动字体尺寸
        /// </summary>
        public bool tmp_enable_auto_sizing = false;
        [SerializeField]
        /// <summary>
        /// Tmp自动字体 - 最小尺寸
        /// </summary>
        public float tmp_font_size_min = 5;
        [SerializeField]
        /// <summary>
        /// Tmp自动字体 - 最小尺寸 - 全局缩放倍增值
        /// </summary>
        public float tmp_FontSizeMin_globalscaled = 5;
        [SerializeField]
        /// <summary>
        /// Tmp自动字体 - 最大尺寸
        /// </summary>
        public float tmp_font_size_max = 100;
        [SerializeField]
        /// <summary>
        /// Tmp自动字体 - 最大尺寸 - 全局缩放倍增值
        /// </summary>
        public float tmp_FontSizeMax_globalscaled = 100;
        [SerializeField]
        /// <summary>
        /// Tmp自动字体 - 宽度百分比
        /// </summary>
        public float tmp_char_width_max_adj;
        [SerializeField]
        /// <summary>
        /// Tmp自动字体 - 最大行高矫正
        /// </summary>
        public float tmp_line_spacing_max;
        //----------------------------------------Tmp Text GradientColor----------------------------------------//
        [SerializeField]
        /// <summary>
        /// 渐变色 A
        /// </summary>
        public Color gra_a = Color.white;
        [SerializeField]
        /// <summary>
        /// 渐变色 B
        /// </summary>
        public Color gra_b = Color.white;
        [SerializeField]
        /// <summary>
        /// 渐变色 C
        /// </summary>
        public Color gra_c = Color.white;
        [SerializeField]
        /// <summary>
        /// 渐变色 D
        /// </summary>
        public Color gra_d = Color.white;
        [SerializeField]
        /// <summary>
        /// 渐变色反转
        /// </summary>
        public bool gra_invert;
        [SerializeField]
        /// <summary>
        /// 渐变色开关
        /// </summary>
        public bool gra_used;
        [SerializeField]
        /// <summary>
        /// 渐变色模式
        /// </summary>
        public ColorMode gra_color_mode;
        [SerializeField]
        /// <summary>
        /// 渐变色模式名称
        /// </summary>
        public string gra_color_mode_name;
        //----------------------------------------Library Toggles----------------------------------------//
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 几何对齐
        /// </summary>
        public bool lib_style_effect_geometre_align;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 对齐
        /// </summary>
        public bool lib_style_effect_align;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 字体
        /// </summary>
        public bool lib_style_effect_font;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 样式
        /// </summary>
        public bool lib_style_effect_style;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 尺寸_Size
        /// </summary>
        public bool lib_style_effect_size;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 颜色_Color
        /// </summary>
        public bool lib_style_effect_color;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 富文本
        /// </summary>
        public bool lib_style_effect_rich;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 行高
        /// </summary>
        public bool lib_style_effect_line;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 溢出水平
        /// </summary>
        public bool lib_style_effect_overflow_h;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 溢出垂直
        /// </summary>
        public bool lib_style_effect_overflow_v;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 溢出包裹
        /// </summary>
        public bool lib_style_effect_wrap;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 包裹比例
        /// </summary>
        public bool lib_style_effect_wrapratio;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 溢出模式
        /// </summary>
        public bool lib_style_effect_overflow;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 边距
        /// </summary>
        public bool lib_style_effect_margin;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 间距
        /// </summary>
        public bool lib_style_effect_space;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 最佳匹配
        /// </summary>
        public bool lib_style_effect_bestfit;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 渐变色
        /// </summary>
        public bool lib_style_effect_gradientcolor;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 自动字体尺寸
        /// </summary>
        public bool lib_style_effect_autosizing;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 射线检测
        /// </summary>
        public bool lib_style_effect_raycast;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 遮罩
        /// </summary>
        public bool lib_style_effect_maskable;
        //----------------------------------------Features----------------------------------------//
        [SerializeField]
        /// <summary>
        /// 射线检测
        /// </summary>
        public bool raycast;
        [SerializeField]
        /// <summary>
        /// 蒙版
        /// </summary>
        public bool maskable;
        #endregion

        /// <summary>
        /// 拷贝数据（主要用于脱离引用的实例化）
        /// </summary>
        /// <param name="target"></param>
        public void CopyData(XHud_LibraryArg_TextStyle target)
        {
            //----------------------------------------Based----------------------------------------//
            name = target.name;
            description = target.description;
            type = target.type;
            //----------------------------------------Text----------------------------------------//
            style = target.style;
            content_anchor = target.content_anchor;
            geometre_align = target.geometre_align;
            font = target.font;
            font_asset_path = target.font_asset_path;
            size = target.size;
            line_height = target.line_height;
            overflow_horizon = target.overflow_horizon;
            overflow_vertical = target.overflow_vertical;
            rich_text = target.rich_text;
            best_fit = target.best_fit;
            fit_min = target.fit_min;
            fit_max = target.fit_max;
            font_color = target.font_color;
            //----------------------------------------Tmp Text----------------------------------------//
            tmp_rich = target.tmp_rich;
            tmp_contentwrap = target.tmp_contentwrap;
            tmp_overflow = target.tmp_overflow;
            tmp_style = target.tmp_style;
            tmp_anchor = target.tmp_anchor;
            tmp_font = target.tmp_font;
            tmp_fontasset_assetpath = target.tmp_fontasset_assetpath;
            tmp_size = target.tmp_size;
            tmp_size_globalscaled = target.tmp_size_globalscaled;
            tmp_space_character = target.tmp_space_character;
            tmp_space_word = target.tmp_space_word;
            tmp_space_lineheight = target.tmp_space_lineheight;
            tmp_space_paragraph = target.tmp_space_paragraph;
            tmp_wrapping_ratios = target.tmp_wrapping_ratios;
            tmp_content_margin = target.tmp_content_margin;
            tmp_color = target.tmp_color;
            tmp_enable_auto_sizing = target.tmp_enable_auto_sizing;
            tmp_font_size_min = target.tmp_font_size_min;
            tmp_FontSizeMin_globalscaled = target.tmp_FontSizeMin_globalscaled;
            tmp_font_size_max = target.tmp_font_size_max;
            tmp_FontSizeMax_globalscaled = target.tmp_FontSizeMax_globalscaled;
            tmp_char_width_max_adj = target.tmp_char_width_max_adj;
            tmp_line_spacing_max = target.tmp_line_spacing_max;
            //----------------------------------------Tmp Text GradientColor----------------------------------------//
            gra_a = target.gra_a;
            gra_b = target.gra_b;
            gra_c = target.gra_c;
            gra_d = target.gra_d;
            gra_invert = target.gra_invert;
            gra_used = target.gra_used;
            gra_color_mode = target.gra_color_mode;
            gra_color_mode_name = target.gra_color_mode_name;
            //----------------------------------------Library Toggles----------------------------------------//
            lib_style_effect_align = target.lib_style_effect_align;
            lib_style_effect_geometre_align = target.lib_style_effect_geometre_align;
            lib_style_effect_font = target.lib_style_effect_font;
            lib_style_effect_style = target.lib_style_effect_style;
            lib_style_effect_size = target.lib_style_effect_size;
            lib_style_effect_color = target.lib_style_effect_color;
            lib_style_effect_rich = target.lib_style_effect_rich;
            lib_style_effect_line = target.lib_style_effect_line;
            lib_style_effect_overflow_h = target.lib_style_effect_overflow_h;
            lib_style_effect_overflow_v = target.lib_style_effect_overflow_v;
            lib_style_effect_wrap = target.lib_style_effect_wrap;
            lib_style_effect_wrapratio = target.lib_style_effect_wrapratio;
            lib_style_effect_overflow = target.lib_style_effect_overflow;
            lib_style_effect_margin = target.lib_style_effect_margin;
            lib_style_effect_space = target.lib_style_effect_space;
            lib_style_effect_bestfit = target.lib_style_effect_bestfit;
            lib_style_effect_gradientcolor = target.lib_style_effect_gradientcolor;
            lib_style_effect_autosizing = target.lib_style_effect_autosizing;
            lib_style_effect_raycast = target.lib_style_effect_raycast;
            lib_style_effect_maskable = target.lib_style_effect_maskable;
            //----------------------------------------Features----------------------------------------//
            raycast = target.raycast;
            maskable = target.maskable;
        }

        /// <summary>
        /// 拷贝数据忽略名称和说明文字（主要用于脱离引用的实例化）
        /// </summary>
        /// <param name="target"></param>
        public void CopyData_WithoutType(XHud_LibraryArg_TextStyle target)
        {
            //----------------------------------------Based----------------------------------------//
            type = target.type;
            //----------------------------------------Text----------------------------------------//
            style = target.style;
            content_anchor = target.content_anchor;
            geometre_align = target.geometre_align;
            font = target.font;
            font_asset_path = target.font_asset_path;
            size = target.size;
            line_height = target.line_height;
            overflow_horizon = target.overflow_horizon;
            overflow_vertical = target.overflow_vertical;
            rich_text = target.rich_text;
            best_fit = target.best_fit;
            fit_min = target.fit_min;
            fit_max = target.fit_max;
            font_color = target.font_color;
            //----------------------------------------Tmp Text----------------------------------------//
            tmp_rich = target.tmp_rich;
            tmp_contentwrap = target.tmp_contentwrap;
            tmp_overflow = target.tmp_overflow;
            tmp_style = target.tmp_style;
            tmp_anchor = target.tmp_anchor;
            tmp_font = target.tmp_font;
            tmp_fontasset_assetpath = target.tmp_fontasset_assetpath;
            tmp_size = target.tmp_size;
            tmp_size_globalscaled = target.tmp_size_globalscaled;
            tmp_space_character = target.tmp_space_character;
            tmp_space_word = target.tmp_space_word;
            tmp_space_lineheight = target.tmp_space_lineheight;
            tmp_space_paragraph = target.tmp_space_paragraph;
            tmp_wrapping_ratios = target.tmp_wrapping_ratios;
            tmp_content_margin = target.tmp_content_margin;
            tmp_color = target.tmp_color;
            tmp_enable_auto_sizing = target.tmp_enable_auto_sizing;
            tmp_font_size_min = target.tmp_font_size_min;
            tmp_FontSizeMin_globalscaled = target.tmp_FontSizeMin_globalscaled;
            tmp_font_size_max = target.tmp_font_size_max;
            tmp_FontSizeMax_globalscaled = target.tmp_FontSizeMax_globalscaled;
            tmp_char_width_max_adj = target.tmp_char_width_max_adj;
            tmp_line_spacing_max = target.tmp_line_spacing_max;
            //----------------------------------------Tmp Text GradientColor----------------------------------------//
            gra_a = target.gra_a;
            gra_b = target.gra_b;
            gra_c = target.gra_c;
            gra_d = target.gra_d;
            gra_invert = target.gra_invert;
            gra_used = target.gra_used;
            gra_color_mode = target.gra_color_mode;
            gra_color_mode_name = target.gra_color_mode_name;
            //----------------------------------------Library Toggles----------------------------------------//
            lib_style_effect_align = target.lib_style_effect_align;
            lib_style_effect_geometre_align = target.lib_style_effect_geometre_align;
            lib_style_effect_font = target.lib_style_effect_font;
            lib_style_effect_style = target.lib_style_effect_style;
            lib_style_effect_size = target.lib_style_effect_size;
            lib_style_effect_color = target.lib_style_effect_color;
            lib_style_effect_rich = target.lib_style_effect_rich;
            lib_style_effect_line = target.lib_style_effect_line;
            lib_style_effect_overflow_h = target.lib_style_effect_overflow_h;
            lib_style_effect_overflow_v = target.lib_style_effect_overflow_v;
            lib_style_effect_wrap = target.lib_style_effect_wrap;
            lib_style_effect_wrapratio = target.lib_style_effect_wrapratio;
            lib_style_effect_overflow = target.lib_style_effect_overflow;
            lib_style_effect_margin = target.lib_style_effect_margin;
            lib_style_effect_space = target.lib_style_effect_space;
            lib_style_effect_bestfit = target.lib_style_effect_bestfit;
            lib_style_effect_gradientcolor = target.lib_style_effect_gradientcolor;
            lib_style_effect_autosizing = target.lib_style_effect_autosizing;
            lib_style_effect_raycast = target.lib_style_effect_raycast;
            lib_style_effect_maskable = target.lib_style_effect_maskable;
            //----------------------------------------Features----------------------------------------//
            raycast = target.raycast;
            maskable = target.maskable;
        }

        /// <summary>
        /// 对比类
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool EqualsData(XHud_LibraryArg_TextStyle info)
        {
            if (info == null)
            {
                return false;
            }

            XHud_LibraryArg_TextStyle s_info = new XHud_LibraryArg_TextStyle();
            s_info.CopyData_WithoutType(this);

            XHud_LibraryArg_TextStyle v_info = new XHud_LibraryArg_TextStyle();
            v_info.CopyData_WithoutType(info);

            string s = JsonUtility.ToJson(s_info);
            string v = JsonUtility.ToJson(v_info);

            if (s == v)
                return true;
            else
                return false;

        }

        /// <summary>
        /// 拷贝数据（主要用于脚本与库数据传递，所以忽略库开关属性）
        /// </summary>
        /// <param name="target"></param>
        public void CopyData_Ignored_LibraryToggle(XHud_LibraryArg_TextStyle target)
        {
            //----------------------------------------Based----------------------------------------//
            name = target.name;
            description = target.description;
            type = target.type;
            //----------------------------------------Text----------------------------------------//
            style = target.style;
            content_anchor = target.content_anchor;
            geometre_align = target.geometre_align;
            font = target.font;
            font_asset_path = target.font_asset_path;
            size = target.size;
            line_height = target.line_height;
            overflow_horizon = target.overflow_horizon;
            overflow_vertical = target.overflow_vertical;
            rich_text = target.rich_text;
            best_fit = target.best_fit;
            fit_min = target.fit_min;
            fit_max = target.fit_max;
            font_color = target.font_color;
            //----------------------------------------Tmp Text----------------------------------------//
            tmp_rich = target.tmp_rich;
            tmp_contentwrap = target.tmp_contentwrap;
            tmp_overflow = target.tmp_overflow;
            tmp_style = target.tmp_style;
            tmp_anchor = target.tmp_anchor;
            tmp_font = target.tmp_font;
            tmp_fontasset_assetpath = target.tmp_fontasset_assetpath;
            tmp_size = target.tmp_size;
            tmp_size_globalscaled = target.tmp_size_globalscaled;
            tmp_space_character = target.tmp_space_character;
            tmp_space_word = target.tmp_space_word;
            tmp_space_lineheight = target.tmp_space_lineheight;
            tmp_space_paragraph = target.tmp_space_paragraph;
            tmp_wrapping_ratios = target.tmp_wrapping_ratios;
            tmp_content_margin = target.tmp_content_margin;
            tmp_color = target.tmp_color;
            tmp_enable_auto_sizing = target.tmp_enable_auto_sizing;
            tmp_font_size_min = target.tmp_font_size_min;
            tmp_FontSizeMin_globalscaled = target.tmp_FontSizeMin_globalscaled;
            tmp_font_size_max = target.tmp_font_size_max;
            tmp_FontSizeMax_globalscaled = target.tmp_FontSizeMax_globalscaled;
            tmp_char_width_max_adj = target.tmp_char_width_max_adj;
            tmp_line_spacing_max = target.tmp_line_spacing_max;
            //----------------------------------------Tmp Text GradientColor----------------------------------------//
            gra_a = target.gra_a;
            gra_b = target.gra_b;
            gra_c = target.gra_c;
            gra_d = target.gra_d;
            gra_invert = target.gra_invert;
            gra_used = target.gra_used;
            gra_color_mode = target.gra_color_mode;
            gra_color_mode_name = target.gra_color_mode_name;
            //----------------------------------------Features----------------------------------------//
            raycast = target.raycast;
            maskable = target.maskable;
        }

        //------------------------通用参数设置

        /// <summary>
        /// 设置射线检测可用性
        /// </summary>
        /// <param name="treeState"></param>
        public void gen_RayCastSet(bool state)
        {
            if (raycast == state)
                return;
            raycast = state;
        }

        /// <summary>
        /// 设置蒙版可用性
        /// </summary>
        /// <param name="treeState"></param>
        public void gen_Set_MaskableSet(bool state)
        {
            if (maskable == state)
                return;
            maskable = state;
        }

        //------------------------Text参数设置

        /// <summary>
        /// 改变字体样式
        /// </summary>
        /// <param name="style">样式</param>
        public void txt_Set_FontStyle(FontStyle style)
        {
            if (this.style == style)
                return;
            this.style = style;
        }

        /// <summary>
        /// 改变文字锚点位置
        /// </summary>
        /// <param name="anchor">锚点位置_AnchoredPosition</param>
        public void txt_Set_Alignment(ContentAnchor anchor)
        {
            if (content_anchor == anchor)
                return;
            content_anchor = anchor;
        }

        /// <summary>
        /// 改变文字几何对齐
        /// </summary>
        /// <param name="treeState"></param>
        public void txt_Set_GeometreAlign(bool state)
        {
            if (geometre_align == state)
                return;
            geometre_align = state;
        }

        /// <summary>
        /// 改变字体
        /// </summary>
        /// <param name="font">字体</param>
        public void txt_Set_Font(Font font)
        {
            if (font == null)
                return;
            if (font == this.font)
                return;
            this.font = font;
        }

        /// <summary>
        /// 改变字体尺寸
        /// </summary>
        /// <param name="size">尺寸_Size</param>
        public void txt_Set_FontSize(float size)
        {
            if (this.size == size)
                return;
            this.size = size;
        }

        /// <summary>
        /// 改变字体行高
        /// </summary>
        /// <param name="height">尺寸_Size</param>
        public void txt_Set_FontLineHeight(float height)
        {
            if (line_height == height)
                return;
            line_height = height;
        }

        /// <summary>
        /// 溢出 H
        /// </summary>
        /// <param name="mode"> Overflow = 溢出, Wrap = 包裹</param>
        public void txt_Set_Overflow(HorizontalWrapMode mode)
        {
            if (overflow_horizon == mode)
                return;
            overflow_horizon = mode;
        }

        /// <summary>
        /// 溢出 V
        /// </summary>
        /// <param name="mode"> Overflow = 溢出, Truncate = 截断</param>
        public void txt_Set_Overflow(VerticalWrapMode mode)
        {
            if (overflow_vertical == mode)
                return;
            overflow_vertical = mode;
        }

        /// <summary>
        /// 设置富文本开关
        /// </summary>
        /// <param name="treeState"></param>
        public void txt_Set_RichTextEnabled(bool state)
        {
            if (rich_text == state)
                return;
            rich_text = state;
        }

        /// <summary>
        /// 设置自动尺寸开关
        /// </summary>
        /// <param name="treeState"></param>
        public void txt_Set_BestFit(bool state)
        {
            if (best_fit == state)
                return;
            best_fit = state;
        }

        /// <summary>
        /// 改变字体自动尺寸最小
        /// </summary>
        /// <param name="size">尺寸_Size</param>
        public void txt_Set_FontFitSize_Min(int size)
        {
            if (fit_min == size)
                return;
            fit_min = size;
        }

        /// <summary>
        /// 改变字体自动尺寸最大
        /// </summary>
        /// <param name="size">尺寸_Size</param>
        public void txt_Set_FontFitSize_Max(int size)
        {
            if (fit_max == size)
                return;
            fit_max = size;
        }

        /// <summary>
        /// 改变字体颜色
        /// </summary>
        /// <param name="color">颜色_Color</param>
        public void txt_Set_FontColor(Color color)
        {
            if (font_color == color)
                return;
            font_color = color;
        }

        //------------------------TmpText参数设置

        /// <summary>
        /// 设置富文本开关
        /// </summary>
        /// <param name="treeState"></param>
        public void tmp_Set_RichTextEnabled(bool state)
        {
            if (tmp_rich == state)
                return;
            tmp_rich = state;
        }

        /// <summary>
        /// 包裹
        /// </summary>
        /// <param name="mode">NoWrap = 不包裹, Normal = 常规包裹, PreserveWhitespace = 保留空格, PreserveWhitespaceNoWrap = 保留空格但不包裹 </param>
        public void tmp_Set_WordWrappingMode(TextWrappingModes mode)
        {
            if (tmp_contentwrap == mode)
                return;
            tmp_contentwrap = mode;
        }

        /// <summary>
        /// 溢出
        /// </summary>
        /// <param name="mode"> Overflow = 溢出, Ellipsis = 省略, Masking = 遮罩, Truncate = 截断, ScrollRect = 滚动区域, Page = 翻页, Linked = 链接</param>
        public void tmp_Set_Overflow(TextOverflowModes mode)
        {
            if (tmp_overflow == mode)
                return;
            tmp_overflow = mode;
        }

        /// <summary>
        /// 改变字体样式
        /// </summary>
        /// <param name="style">样式</param>
        public void tmp_Set_FontStyle(FontStyles style)
        {
            if (tmp_style == style)
                return;
            tmp_style = style;
        }

        /// <summary>
        /// 改变文字锚点位置
        /// </summary>
        /// <param name="anchor">锚点位置_AnchoredPosition</param>
        public void tmp_Set_Alignment(TmpContentAnchor anchor)
        {
            if (tmp_anchor != anchor)
                tmp_anchor = anchor;
        }

        /// <summary>
        /// 改变字体
        /// </summary>
        /// <param name="font">字体</param>
        public void tmp_Set_FontAsset(TMP_FontAsset font)
        {
            if (font == null)
                return;
            if (font == tmp_font)
                return;
            tmp_font = font;
        }

        /// <summary>
        /// 改变字体尺寸
        /// </summary>
        /// <param name="size">尺寸_Size</param>
        public void tmp_Set_FontSize(float size)
        {
            if (tmp_size == size)
                return;
            tmp_size = size;
        }

        /// <summary>
        /// 字符间距
        /// </summary>
        /// <param name="distance">距离</param>
        public void tmp_Set_SpacingSet_Character(float value)
        {
            if (tmp_space_character == value)
                return;
            tmp_space_character = value;
        }

        /// <summary>
        /// 单词间距
        /// </summary>
        /// <param name="distance">距离</param>
        public void tmp_Set_SpacingSet_Word(float value)
        {
            if (tmp_space_word == value)
                return;
            tmp_space_word = value;
        }

        /// <summary>
        /// 行高
        /// </summary>
        /// <param name="distance">距离</param>
        public void tmp_Set_SpacingSet_Line(float value)
        {
            if (tmp_space_lineheight == value)
                return;
            tmp_space_lineheight = value;
        }

        /// <summary>
        /// 段落间距
        /// </summary>
        /// <param name="distance">距离</param>
        public void tmp_Set_SpacingSet_Paragraph(float value)
        {
            if (tmp_space_paragraph == value)
                return;
            tmp_space_paragraph = value;
        }

        /// <summary>
        /// 设置文字包裹比例
        /// </summary>
        /// <param name="val"></param>
        public void tmp_Set_WrappingRatios(float val)
        {
            if (tmp_wrapping_ratios == val)
                return;
            tmp_wrapping_ratios = val;
        }

        /// <summary>
        /// 边距值设置
        /// </summary>
        /// <param name="value">边距值</param>
        public void tmp_Set_MarginSet(Vector4 value)
        {
            if (tmp_content_margin == value)
                return;
            tmp_content_margin = value;
        }

        /// <summary>
        /// 间距值设置
        /// </summary>
        /// <param name="values">间距值</param>
        public void tmp_Set_SpacingSet(XHud_TmpTextSpacingValue values)
        {
            if (values.space_Character != tmp_space_character)
            {
                tmp_space_character = values.space_Character;
            }
            if (values.space_Word != tmp_space_word)
            {
                tmp_space_word = values.space_Word;
            }
            if (values.space_Line != tmp_space_lineheight)
            {
                tmp_space_lineheight = values.space_Line;
            }
            if (values.space_Paragraph != tmp_space_paragraph)
            {
                tmp_space_paragraph = values.space_Paragraph;
            }
        }

        /// <summary>
        /// 改变字体颜色
        /// </summary>
        /// <param name="color">颜色_Color</param>
        public void tmp_Set_FontColor(Color color)
        {
            if (tmp_color == color)
                return;
            tmp_color = color;
        }

        /// <summary>
        /// 最佳匹配尺寸开启设置
        /// </summary>
        public void tmp_Set_EnableAutoSizing(bool state)
        {
            if (tmp_enable_auto_sizing == state)
                return;
            tmp_enable_auto_sizing = state;
        }

        /// <summary>
        /// 最佳匹配尺寸设置
        /// </summary>
        public void tmp_Set_FontAutoSize(Vector4 autosize)
        {
            if (autosize.x != tmp_font_size_min)
            {
                tmp_font_size_min = autosize.x;
            }
            if (autosize.y != tmp_font_size_max)
            {
                tmp_font_size_max = autosize.y;
            }
            if (autosize.z != tmp_char_width_max_adj)
            {
                tmp_char_width_max_adj = autosize.z;
            }
            if (autosize.w != tmp_line_spacing_max)
            {
                tmp_line_spacing_max = autosize.w;
            }
        }

        /// <summary>
        /// 最佳匹配尺寸设置 - 最小尺寸
        /// </summary>
        public void tmp_Set_FontAutoSize_FontSizeMin(float value)
        {
            if (value != tmp_font_size_min)
            {
                tmp_font_size_min = value;
            }
        }

        /// <summary>
        /// 最佳匹配尺寸设置 - 最大尺寸
        /// </summary>
        public void tmp_Set_FontAutoSize_FontSizeMax(float value)
        {
            if (value != tmp_font_size_max)
            {
                tmp_font_size_max = value;
            }
        }

        /// <summary>
        /// 最佳匹配尺寸设置 - 字符宽度百分比
        /// </summary>
        public void tmp_Set_FontAutoSize_CharWidthMaxAdj(float value)
        {
            if (value != tmp_char_width_max_adj)
            {
                tmp_char_width_max_adj = value;
            }
        }

        /// <summary>
        /// 最佳匹配尺寸设置 - 最大行高
        /// </summary> 
        public void tmp_Set_FontAutoSize_LineSpacingMax(float value)
        {
            if (value != tmp_line_spacing_max)
            {
                tmp_line_spacing_max = value;
            }
        }

        /// <summary>
        /// 设置渐变色 A
        /// </summary>
        /// <param name="col"></param>
        public void tmp_Set_GradientColor_A(Color col)
        {
            if (gra_a == col)
                return;
            gra_a = col;
        }

        /// <summary>
        /// 设置渐变色 B
        /// </summary>
        /// <param name="col"></param>
        public void tmp_Set_GradientColor_B(Color col)
        {
            if (gra_b == col)
                return;
            gra_b = col;
        }

        /// <summary>
        /// 设置渐变色 C
        /// </summary>
        /// <param name="col"></param>
        public void tmp_Set_GradientColor_C(Color col)
        {
            if (gra_c == col)
                return;
            gra_c = col;
        }

        /// <summary>
        /// 设置渐变色 D
        /// </summary>
        /// <param name="col"></param>
        public void tmp_Set_GradientColor_D(Color col)
        {
            if (gra_d == col)
                return;
            gra_d = col;
        }

        /// <summary>
        /// 设置渐变色反转
        /// </summary>
        /// <param name="treeState"></param>
        public void tmp_Set_GradientColor_Invert(bool state)
        {
            if (gra_invert == state)
                return;
            gra_invert = state;
        }

        /// <summary>
        /// 设置渐变色开关
        /// </summary>
        /// <param name="treeState"></param>
        public void tmp_Set_GradientColor_Enabled(bool state)
        {
            if (gra_used == state)
                return;
            gra_used = state;
        }

        /// <summary>
        /// 设置渐变色模式
        /// </summary>
        /// <param name="col"></param>
        public void tmp_Set_GradientMode(ColorMode mode)
        {
            if (gra_color_mode == mode)
                return;
            gra_color_mode = mode;
        }

        /// <summary>
        /// 设置渐变色模式名称
        /// </summary>
        /// <param name="col"></param>
        public void tmp_Set_GradientModeName(string mode)
        {
            if (gra_color_mode_name == mode)
                return;
            gra_color_mode_name = mode;
        }
    }

    [CreateAssetMenu(fileName = "XHud_Library_TextStyle", menuName = "XHud/CreateAssets (创建Hud资源库)/Library-TextStyle (文字样式库)", order = 0)]
    public class XHud_Library_TextStyle : ScriptableObject
    {
        #region 字段 - 库基础信息
        /// <summary>
        /// 文字样式库名称
        /// </summary>
        public string library_name = "NewTextStyleLibrary";
        /// <summary>
        /// 文字样式库数据列表
        /// </summary>
        public List<XHud_LibraryArg_TextStyle> library_text_styles = new List<XHud_LibraryArg_TextStyle>();
        /// <summary>
        /// 文字样式变化时触发的事件
        /// </summary>
        public UnityAction act_on_text_style_changed;
        #endregion

        #region 字段 - 预览设置
        /// <summary>
        /// 预览字体
        /// </summary>
        public Font preview_font;
        /// <summary>
        /// 预览字体颜色
        /// </summary>
        public Color preview_font_color = Color.white;
        /// <summary>
        /// 预览内容
        /// </summary>
        public string preview_content = "XHud Sample Fonts";
        /// <summary>
        /// 预览文本类型
        /// </summary>
        public string preview_type_text = "Text";
        /// <summary>
        /// 预览字体样式
        /// </summary>
        public FontStyle preview_style = FontStyle.Normal;
        #endregion

        #region 字段 - 列表查询与筛选
        /// <summary>
        /// 名称项查找（精确匹配）
        /// </summary>
        public string find = "";
        /// <summary>
        /// Editor 筛选名称
        /// </summary>
        public string highlight;
        #endregion

        #region 字段 - 列表布局
        /// <summary>
        /// Editor 列表项高度
        /// </summary>
        public float item_height = 61;
        /// <summary>
        /// 可视区域显示的元素数量
        /// </summary>
        public int visible_item_count = 7;
        /// <summary>
        /// 列表滚动位置
        /// </summary>
        public Vector2 color_info_list_original_scroller;
        #endregion

        #region 字段 - 选中与定位
        /// <summary>
        /// 定位选中项索引号（-1 表示无）
        /// </summary>
        public int location_selected_index = -1;
        /// <summary>
        /// 选中项索引号
        /// </summary>
        public int selected_index;
        #endregion

        private void OnEnable()
        {
            item_height = 61;
            visible_item_count = 7;
        }

        // 确保内部名称与文件名一致
        private void OnValidate()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(name))
            {
                string path = AssetDatabase.GetAssetPath(this);
                if (!string.IsNullOrEmpty(path))
                {
                    name = Path.GetFileNameWithoutExtension(path);
                }
            }
#endif
        }

        /// <summary>
        /// 获取字体库数量
        /// </summary>
        /// <returns></returns>
        public int TextStyle_Library_GetCount()
        {
            return library_text_styles.Count;
        }

        /// <summary>
        /// 获取字体库数量
        /// </summary>
        /// <returns></returns>
        public int TextStyle_Library_GetCount(xHud_TextType type)
        {
            int count = 0;
            for (int i = 0; i < library_text_styles.Count; i++)
            {
                if (library_text_styles[i].type == type)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 检查字体库是否为空
        /// </summary>
        /// <returns></returns>
        public bool TextStyle_Library_IsEmpty()
        {
            if (library_text_styles != null)
            {
                if (library_text_styles.Count > 0)
                    return false;
                else
                    return true;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 检查字体库是否重复
        /// </summary>
        /// <returns></returns>
        public bool TextStyle_Library_IsRepeat(string name)
        {
            if (TextStyle_Library_IsEmpty())
            {
                Debug.Log("请先确保字体库不为空！");
                return false;
            }
            bool isrep = false;
            for (int i = 0; i < library_text_styles.Count; i++)
            {
                if (name == library_text_styles[i].name)
                    isrep = true;
            }
            return isrep;
        }

        /// <summary>
        /// 替换字体库样式
        /// </summary>
        /// <returns></returns>
        public void TextStyle_Library_Replace(int index, XHud_LibraryArg_TextStyle info)
        {
            library_text_styles[index].CopyData_Ignored_LibraryToggle(info);
        }

        /// <summary>
        /// 字体库移除目标样式
        /// </summary>
        /// <returns></returns>
        public void TextStyle_Library_Remove(string name)
        {
            if (TextStyle_Library_IsEmpty())
            {
                Debug.Log("请先确保字体库不为空！");
                return;
            }
            bool ismatched = false;
            int xc = 0;
            for (int i = 0; i < library_text_styles.Count; i++)
            {
                if (name == library_text_styles[i].name)
                {
                    xc = i;
                    ismatched = true;
                }
            }
            if (ismatched)
                library_text_styles.RemoveAt(xc);

            if (act_on_text_style_changed != null)
                act_on_text_style_changed();
        }

        /// <summary>
        /// 添加一个字体样式模版
        /// </summary>
        /// <param name="info">文字样式信息类</param>
        public void TextStyle_Library_Add(XHud_LibraryArg_TextStyle info)
        {
            library_text_styles.Add(info);
            if (act_on_text_style_changed != null)
                act_on_text_style_changed();
        }

        /// <summary>
        /// 获取字体库所有样式
        /// </summary>
        /// <returns></returns>
        public XHud_LibraryArg_TextStyle[] TextStyle_Library_GetAllStyle()
        {
            return library_text_styles.ToArray();
        }

        /// <summary>
        /// 获取字体库所有名称
        /// </summary>
        /// <returns></returns>
        public string[] TextStyle_Library_GetAllNames()
        {
            string[] names = new string[library_text_styles.Count];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = library_text_styles[i].name;
            }
            return names;
        }

        /// <summary>
        /// 获取字体库所有样式 - Text
        /// </summary>
        /// <returns></returns>
        public XHud_LibraryArg_TextStyle[] TextStyle_Library_GetAllStyle_With_Text()
        {
            List<XHud_LibraryArg_TextStyle> info = new List<XHud_LibraryArg_TextStyle>();
            for (int i = 0; i < library_text_styles.Count; i++)
            {
                if (library_text_styles[i].type == xHud_TextType.Text)
                    info.Add(library_text_styles[i]);
            }
            return info.ToArray();
        }

        /// <summary>
        /// 获取字体库所有样式名称 - Text
        /// </summary>
        /// <returns></returns>
        public string[] TextStyle_Library_GetAllNames_With_Text()
        {
            List<string> info = new List<string>();
            for (int i = 0; i < library_text_styles.Count; i++)
            {
                if (library_text_styles[i].type == xHud_TextType.Text)
                    info.Add(library_text_styles[i].name);
            }
            return info.ToArray();
        }

        /// <summary>
        /// 获取字体库所有样式 - TmpText
        /// </summary>
        /// <returns></returns>
        public XHud_LibraryArg_TextStyle[] TextStyle_Library_GetAllStyle_With_TmpText()
        {
            List<XHud_LibraryArg_TextStyle> info = new List<XHud_LibraryArg_TextStyle>();
            for (int i = 0; i < library_text_styles.Count; i++)
            {
                if (library_text_styles[i].type == xHud_TextType.TmpText)
                    info.Add(library_text_styles[i]);
            }
            return info.ToArray();
        }

        /// <summary>
        /// 获取字体库所有样式名称 - TmpText
        /// </summary>
        /// <returns></returns>
        public string[] TextStyle_Library_GetAllNames_With_TmpText()
        {
            List<string> info = new List<string>();
            for (int i = 0; i < library_text_styles.Count; i++)
            {
                if (library_text_styles[i].type == xHud_TextType.TmpText)
                    info.Add(library_text_styles[i].name);
            }
            return info.ToArray();
        }

        /// <summary>
        /// 获取目标ID的字体样式
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public XHud_LibraryArg_TextStyle TextStyle_Library_GetTextStyleInfo(int index)
        {
            if (index < library_text_styles.Count)
                return library_text_styles[index];
            else
                return library_text_styles[0];
        }

        /// <summary>
        /// 根据目标Index和类型获取字体样式
        /// </summary>
        /// <param name="index"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public XHud_LibraryArg_TextStyle TextStyle_Library_GetTextStyleInfo(int index, xHud_TextType type)
        {
            if (index < library_text_styles.Count)
                if (library_text_styles[index].type == type)
                    return library_text_styles[index];
                else
                    return null;
            else
                return library_text_styles[0];
        }

        /// <summary>
        /// 获取目标名称的字体样式
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XHud_LibraryArg_TextStyle TextStyle_Library_GetTextStyleInfo(string name)
        {
            XHud_LibraryArg_TextStyle info = null;
            for (int i = 0; i < library_text_styles.Count; i++)
            {
                if (library_text_styles[i].name == name)
                {
                    info = library_text_styles[i];
                }
            }
            return info;
        }

        /// <summary>
        /// 根据目标名称和类型获取字体样式
        /// </summary>
        /// <param name="index"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public XHud_LibraryArg_TextStyle TextStyle_Library_GetTextStyleInfo(string name, xHud_TextType type)
        {
            XHud_LibraryArg_TextStyle info = null;
            for (int i = 0; i < library_text_styles.Count; i++)
            {
                if (library_text_styles[i].name == name && library_text_styles[i].type == type)
                {
                    info = library_text_styles[i];
                }
            }
            return info;
        }

        /// <summary>
        /// 获取首个目标类型的字体样式
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public XHud_LibraryArg_TextStyle TextStyle_Library_GetFirstStyleInfo_With_Type(xHud_TextType type)
        {
            List<XHud_LibraryArg_TextStyle> infos = new List<XHud_LibraryArg_TextStyle>();
            for (int i = 0; i < library_text_styles.Count; i++)
            {
                if (library_text_styles[i].type == type)
                    infos.Add(library_text_styles[i]);
            }
            return infos[0];
        }

        /// <summary>
        /// 样式是否存在于库中
        /// </summary>
        /// <returns></returns>
        public bool TextStyle_Library_NameIsValid(string name)
        {
            bool sw = false;
            for (int i = 0; i < library_text_styles.Count; i++)
            {
                if (library_text_styles[i].name == name)
                {
                    sw = true;
                    break;
                }
            }
            return sw;
        }

        /// <summary>
        /// 样式是否存在于库中
        /// </summary>
        /// <returns></returns>
        public bool TextStyle_Library_NameIsValid(string name, xHud_TextType type)
        {
            bool sw = false;
            for (int i = 0; i < library_text_styles.Count; i++)
            {
                if (library_text_styles[i].name == name && library_text_styles[i].type == type)
                {
                    sw = true;
                    break;
                }
            }
            return sw;
        }

        /// <summary>
        /// 根据名称获取索引号
        /// </summary>
        /// <returns></returns>
        public int TextStyle_Library_GetIndex(string name)
        {
            int index = 0;

            for (int i = 0; i < library_text_styles.Count; i++)
            {
                if (library_text_styles[i].name == name)
                    index = i;
            }

            return index;
        }

        /// <summary>
        /// 列表定位并滚动到目标 - 定位
        /// </summary>
        /// <param name="name"></param>
        public void TextStyleLibrary_Location(string name)
        {
            int index = TextStyle_Library_GetIndex(name);
            selected_index = index;
            location_selected_index = index;
            highlight = name;

            //计算列表滚动值
            float scrollval = item_height;
            for (int i = 0; i < library_text_styles.Count; i++)
            {
                if (library_text_styles[i].name == name)
                {
                    if (i > visible_item_count / 2)
                        scrollval -= (visible_item_count / 2) * item_height;
                    else
                        scrollval -= item_height;
                    break;
                }
                else
                {
                    scrollval += item_height;
                }
            }
            Vector2 newscroll = color_info_list_original_scroller;
            newscroll.y = scrollval;
            color_info_list_original_scroller = newscroll;
        }

        /// <summary>
        /// 列表定位并滚动到目标 - 查找
        /// </summary>
        /// <param name="name"></param>
        public void TextStyleLibrary_Location_Find(string name)
        {
            int index = TextStyle_Library_GetIndex(name);
            selected_index = index;
            location_selected_index = index;

            //计算列表滚动值
            float scrollval = item_height;
            for (int i = 0; i < library_text_styles.Count; i++)
            {
                if (library_text_styles[i].name == name)
                {
                    if (i > visible_item_count / 2)
                        scrollval -= (visible_item_count / 2) * item_height;
                    else
                        scrollval -= item_height;
                    break;
                }
                else
                {
                    scrollval += item_height;
                }
            }
            Vector2 newscroll = color_info_list_original_scroller;
            newscroll.y = scrollval;
            color_info_list_original_scroller = newscroll;
        }
    }
}