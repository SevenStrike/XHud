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
        public string Name;
        [SerializeField]
        /// <summary>
        /// 文字模版解释
        /// </summary>
        public string Description;
        [SerializeField]
        /// <summary>
        /// 文字组件类型
        /// </summary>
        public xHud_TextType Type = xHud_TextType.Text;
        //----------------------------------------Text----------------------------------------//
        [SerializeField]
        /// <summary>
        /// 字体样式
        /// </summary>
        public FontStyle Style = FontStyle.Normal;
        [SerializeField]
        /// <summary>
        /// 文字组件锚点
        /// </summary>
        public ContentAnchor ContentAnchor = ContentAnchor.中心;
        [SerializeField]
        /// <summary>
        /// 文字字体
        /// </summary>
        public Font Font;
        [SerializeField]
        /// <summary>
        /// 文字字体路径
        /// </summary>
        public string Font_AssetPath;
        [SerializeField]
        [Range(0, 300)]
        /// <summary>
        /// 文字尺寸
        /// </summary>
        public float Size = 13;
        [SerializeField]
        /// <summary>
        /// 文字行高
        /// </summary>
        public float LineHeight = 1;
        [SerializeField]
        /// <summary>
        /// 水平溢出
        /// </summary>
        public HorizontalWrapMode Overflow_Horizon = HorizontalWrapMode.Wrap;
        [SerializeField]
        /// <summary>
        /// 垂直溢出
        /// </summary>
        public VerticalWrapMode Overflow_Vertical = VerticalWrapMode.Truncate;
        [SerializeField]
        /// <summary>
        /// 富文本支持
        /// </summary>
        public bool RichText = false;
        [SerializeField]
        /// <summary>
        /// 几何对齐
        /// </summary>
        public bool GeometreAlign = false;
        [SerializeField]
        /// <summary>
        /// 自动尺寸
        /// </summary>
        public bool BestFit = false;
        [SerializeField]
        /// <summary>
        /// 自动尺寸最小
        /// </summary>
        public int Fit_Min = 1;
        [SerializeField]
        /// <summary>
        /// 自动字体尺寸 最小 - 全局缩放倍增值
        /// </summary>
        public float Fit_Min_GlobalScaled;
        [SerializeField]
        [Range(0, 300)]
        /// <summary>
        /// 自动尺寸最大
        /// </summary>
        public int Fit_Max = 50;
        [SerializeField]
        /// <summary>
        /// 自动字体尺寸 最大 - 全局缩放倍增值
        /// </summary>
        public float Fit_Max_GlobalScaled;
        [SerializeField]
        /// <summary>
        /// 字体尺寸 - 全局缩放倍增值
        /// </summary>
        public float Size_GlobalScaled;
        [SerializeField]
        /// <summary>
        /// 字体颜色
        /// </summary>
        public Color FontColor = Color.white;
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
        public float tmp_WrappingRatios;
        [SerializeField]
        /// <summary>
        /// Tmp内容边距
        /// </summary>
        public Vector4 tmp_contentmargin;
        [SerializeField]
        /// <summary>
        /// Tmp字体颜色
        /// </summary>
        public Color tmp_color = Color.white;
        [SerializeField]
        /// <summary>
        /// Tmp自动字体尺寸
        /// </summary>
        public bool tmp_EnableAutoSizing = false;
        [SerializeField]
        /// <summary>
        /// Tmp自动字体 - 最小尺寸
        /// </summary>
        public float tmp_FontSizeMin = 5;
        [SerializeField]
        /// <summary>
        /// Tmp自动字体 - 最小尺寸 - 全局缩放倍增值
        /// </summary>
        public float tmp_FontSizeMin_globalscaled = 5;
        [SerializeField]
        /// <summary>
        /// Tmp自动字体 - 最大尺寸
        /// </summary>
        public float tmp_FontSizeMax = 100;
        [SerializeField]
        /// <summary>
        /// Tmp自动字体 - 最大尺寸 - 全局缩放倍增值
        /// </summary>
        public float tmp_FontSizeMax_globalscaled = 100;
        [SerializeField]
        /// <summary>
        /// Tmp自动字体 - 宽度百分比
        /// </summary>
        public float tmp_CharWidthMaxAdj;
        [SerializeField]
        /// <summary>
        /// Tmp自动字体 - 最大行高矫正
        /// </summary>
        public float tmp_LineSpacingMax;
        //----------------------------------------Tmp Text GradientColor----------------------------------------//
        [SerializeField]
        /// <summary>
        /// 渐变色 A
        /// </summary>
        public Color gra_A = Color.white;
        [SerializeField]
        /// <summary>
        /// 渐变色 B
        /// </summary>
        public Color gra_B = Color.white;
        [SerializeField]
        /// <summary>
        /// 渐变色 C
        /// </summary>
        public Color gra_C = Color.white;
        [SerializeField]
        /// <summary>
        /// 渐变色 D
        /// </summary>
        public Color gra_D = Color.white;
        [SerializeField]
        /// <summary>
        /// 渐变色反转
        /// </summary>
        public bool gra_Invert;
        [SerializeField]
        /// <summary>
        /// 渐变色开关
        /// </summary>
        public bool gra_Used;
        [SerializeField]
        /// <summary>
        /// 渐变色模式
        /// </summary>
        public ColorMode gra_ColorMode;
        [SerializeField]
        /// <summary>
        /// 渐变色模式名称
        /// </summary>
        public string gra_ColorModeName;
        //----------------------------------------Library Toggles----------------------------------------//
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 几何对齐
        /// </summary>
        public bool LibStyle_Effect_geometre_align;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 对齐
        /// </summary>
        public bool LibStyle_Effect_align;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 字体
        /// </summary>
        public bool LibStyle_Effect_font;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 样式
        /// </summary>
        public bool LibStyle_Effect_style;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 尺寸_Size
        /// </summary>
        public bool LibStyle_Effect_size;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 颜色_Color
        /// </summary>
        public bool LibStyle_Effect_color;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 富文本
        /// </summary>
        public bool LibStyle_Effect_rich;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 行高
        /// </summary>
        public bool LibStyle_Effect_line;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 溢出水平
        /// </summary>
        public bool LibStyle_Effect_overflow_h;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 溢出垂直
        /// </summary>
        public bool LibStyle_Effect_overflow_v;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 溢出包裹
        /// </summary>
        public bool LibStyle_Effect_wrap;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 包裹比例
        /// </summary>
        public bool LibStyle_Effect_wrapratio;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 溢出模式
        /// </summary>
        public bool LibStyle_Effect_overflow;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 边距
        /// </summary>
        public bool LibStyle_Effect_margin;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 间距
        /// </summary>
        public bool LibStyle_Effect_space;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 最佳匹配
        /// </summary>
        public bool LibStyle_Effect_bestfit;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 渐变色
        /// </summary>
        public bool LibStyle_Effect_gradientcolor;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 自动字体尺寸
        /// </summary>
        public bool LibStyle_Effect_autosizing;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 射线检测
        /// </summary>
        public bool LibStyle_Effect_raycast;
        [SerializeField]
        /// <summary>
        /// 库同步开关 - 遮罩
        /// </summary>
        public bool LibStyle_Effect_maskable;
        //----------------------------------------Features----------------------------------------//
        [SerializeField]
        /// <summary>
        /// 射线检测
        /// </summary>
        public bool Raycast;
        [SerializeField]
        /// <summary>
        /// 蒙版
        /// </summary>
        public bool Maskable;
        [SerializeField]
        /// <summary>
        /// 同步 图元配色器 颜色
        /// </summary>
        public bool SyncPrimitivePaintingColor;
        #endregion

        /// <summary>
        /// 拷贝数据（主要用于脱离引用的实例化）
        /// </summary>
        /// <param name="target"></param>
        public void CopyData(XHud_LibraryArg_TextStyle target)
        {
            //----------------------------------------Based----------------------------------------//
            Name = target.Name;
            Description = target.Description;
            Type = target.Type;
            //----------------------------------------Text----------------------------------------//
            Style = target.Style;
            ContentAnchor = target.ContentAnchor;
            GeometreAlign = target.GeometreAlign;
            Font = target.Font;
            Font_AssetPath = target.Font_AssetPath;
            Size = target.Size;
            LineHeight = target.LineHeight;
            Overflow_Horizon = target.Overflow_Horizon;
            Overflow_Vertical = target.Overflow_Vertical;
            RichText = target.RichText;
            BestFit = target.BestFit;
            Fit_Min = target.Fit_Min;
            Fit_Max = target.Fit_Max;
            FontColor = target.FontColor;
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
            tmp_WrappingRatios = target.tmp_WrappingRatios;
            tmp_contentmargin = target.tmp_contentmargin;
            tmp_color = target.tmp_color;
            tmp_EnableAutoSizing = target.tmp_EnableAutoSizing;
            tmp_FontSizeMin = target.tmp_FontSizeMin;
            tmp_FontSizeMin_globalscaled = target.tmp_FontSizeMin_globalscaled;
            tmp_FontSizeMax = target.tmp_FontSizeMax;
            tmp_FontSizeMax_globalscaled = target.tmp_FontSizeMax_globalscaled;
            tmp_CharWidthMaxAdj = target.tmp_CharWidthMaxAdj;
            tmp_LineSpacingMax = target.tmp_LineSpacingMax;
            //----------------------------------------Tmp Text GradientColor----------------------------------------//
            gra_A = target.gra_A;
            gra_B = target.gra_B;
            gra_C = target.gra_C;
            gra_D = target.gra_D;
            gra_Invert = target.gra_Invert;
            gra_Used = target.gra_Used;
            gra_ColorMode = target.gra_ColorMode;
            gra_ColorModeName = target.gra_ColorModeName;
            //----------------------------------------Library Toggles----------------------------------------//
            LibStyle_Effect_align = target.LibStyle_Effect_align;
            LibStyle_Effect_geometre_align = target.LibStyle_Effect_geometre_align;
            LibStyle_Effect_font = target.LibStyle_Effect_font;
            LibStyle_Effect_style = target.LibStyle_Effect_style;
            LibStyle_Effect_size = target.LibStyle_Effect_size;
            LibStyle_Effect_color = target.LibStyle_Effect_color;
            LibStyle_Effect_rich = target.LibStyle_Effect_rich;
            LibStyle_Effect_line = target.LibStyle_Effect_line;
            LibStyle_Effect_overflow_h = target.LibStyle_Effect_overflow_h;
            LibStyle_Effect_overflow_v = target.LibStyle_Effect_overflow_v;
            LibStyle_Effect_wrap = target.LibStyle_Effect_wrap;
            LibStyle_Effect_wrapratio = target.LibStyle_Effect_wrapratio;
            LibStyle_Effect_overflow = target.LibStyle_Effect_overflow;
            LibStyle_Effect_margin = target.LibStyle_Effect_margin;
            LibStyle_Effect_space = target.LibStyle_Effect_space;
            LibStyle_Effect_bestfit = target.LibStyle_Effect_bestfit;
            LibStyle_Effect_gradientcolor = target.LibStyle_Effect_gradientcolor;
            LibStyle_Effect_autosizing = target.LibStyle_Effect_autosizing;
            LibStyle_Effect_raycast = target.LibStyle_Effect_raycast;
            LibStyle_Effect_maskable = target.LibStyle_Effect_maskable;
            //----------------------------------------Features----------------------------------------//
            Raycast = target.Raycast;
            Maskable = target.Maskable;
            SyncPrimitivePaintingColor = target.SyncPrimitivePaintingColor;
        }

        /// <summary>
        /// 拷贝数据忽略名称和说明文字（主要用于脱离引用的实例化）
        /// </summary>
        /// <param name="target"></param>
        public void CopyData_WithoutType(XHud_LibraryArg_TextStyle target)
        {
            //----------------------------------------Based----------------------------------------//
            Type = target.Type;
            //----------------------------------------Text----------------------------------------//
            Style = target.Style;
            ContentAnchor = target.ContentAnchor;
            GeometreAlign = target.GeometreAlign;
            Font = target.Font;
            Font_AssetPath = target.Font_AssetPath;
            Size = target.Size;
            LineHeight = target.LineHeight;
            Overflow_Horizon = target.Overflow_Horizon;
            Overflow_Vertical = target.Overflow_Vertical;
            RichText = target.RichText;
            BestFit = target.BestFit;
            Fit_Min = target.Fit_Min;
            Fit_Max = target.Fit_Max;
            FontColor = target.FontColor;
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
            tmp_WrappingRatios = target.tmp_WrappingRatios;
            tmp_contentmargin = target.tmp_contentmargin;
            tmp_color = target.tmp_color;
            tmp_EnableAutoSizing = target.tmp_EnableAutoSizing;
            tmp_FontSizeMin = target.tmp_FontSizeMin;
            tmp_FontSizeMin_globalscaled = target.tmp_FontSizeMin_globalscaled;
            tmp_FontSizeMax = target.tmp_FontSizeMax;
            tmp_FontSizeMax_globalscaled = target.tmp_FontSizeMax_globalscaled;
            tmp_CharWidthMaxAdj = target.tmp_CharWidthMaxAdj;
            tmp_LineSpacingMax = target.tmp_LineSpacingMax;
            //----------------------------------------Tmp Text GradientColor----------------------------------------//
            gra_A = target.gra_A;
            gra_B = target.gra_B;
            gra_C = target.gra_C;
            gra_D = target.gra_D;
            gra_Invert = target.gra_Invert;
            gra_Used = target.gra_Used;
            gra_ColorMode = target.gra_ColorMode;
            gra_ColorModeName = target.gra_ColorModeName;
            //----------------------------------------Library Toggles----------------------------------------//
            LibStyle_Effect_align = target.LibStyle_Effect_align;
            LibStyle_Effect_geometre_align = target.LibStyle_Effect_geometre_align;
            LibStyle_Effect_font = target.LibStyle_Effect_font;
            LibStyle_Effect_style = target.LibStyle_Effect_style;
            LibStyle_Effect_size = target.LibStyle_Effect_size;
            LibStyle_Effect_color = target.LibStyle_Effect_color;
            LibStyle_Effect_rich = target.LibStyle_Effect_rich;
            LibStyle_Effect_line = target.LibStyle_Effect_line;
            LibStyle_Effect_overflow_h = target.LibStyle_Effect_overflow_h;
            LibStyle_Effect_overflow_v = target.LibStyle_Effect_overflow_v;
            LibStyle_Effect_wrap = target.LibStyle_Effect_wrap;
            LibStyle_Effect_wrapratio = target.LibStyle_Effect_wrapratio;
            LibStyle_Effect_overflow = target.LibStyle_Effect_overflow;
            LibStyle_Effect_margin = target.LibStyle_Effect_margin;
            LibStyle_Effect_space = target.LibStyle_Effect_space;
            LibStyle_Effect_bestfit = target.LibStyle_Effect_bestfit;
            LibStyle_Effect_gradientcolor = target.LibStyle_Effect_gradientcolor;
            LibStyle_Effect_autosizing = target.LibStyle_Effect_autosizing;
            LibStyle_Effect_raycast = target.LibStyle_Effect_raycast;
            LibStyle_Effect_maskable = target.LibStyle_Effect_maskable;
            //----------------------------------------Features----------------------------------------//
            Raycast = target.Raycast;
            Maskable = target.Maskable;
            SyncPrimitivePaintingColor = target.SyncPrimitivePaintingColor;
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
            Name = target.Name;
            Description = target.Description;
            Type = target.Type;
            //----------------------------------------Text----------------------------------------//
            Style = target.Style;
            ContentAnchor = target.ContentAnchor;
            GeometreAlign = target.GeometreAlign;
            Font = target.Font;
            Font_AssetPath = target.Font_AssetPath;
            Size = target.Size;
            LineHeight = target.LineHeight;
            Overflow_Horizon = target.Overflow_Horizon;
            Overflow_Vertical = target.Overflow_Vertical;
            RichText = target.RichText;
            BestFit = target.BestFit;
            Fit_Min = target.Fit_Min;
            Fit_Max = target.Fit_Max;
            FontColor = target.FontColor;
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
            tmp_WrappingRatios = target.tmp_WrappingRatios;
            tmp_contentmargin = target.tmp_contentmargin;
            tmp_color = target.tmp_color;
            tmp_EnableAutoSizing = target.tmp_EnableAutoSizing;
            tmp_FontSizeMin = target.tmp_FontSizeMin;
            tmp_FontSizeMin_globalscaled = target.tmp_FontSizeMin_globalscaled;
            tmp_FontSizeMax = target.tmp_FontSizeMax;
            tmp_FontSizeMax_globalscaled = target.tmp_FontSizeMax_globalscaled;
            tmp_CharWidthMaxAdj = target.tmp_CharWidthMaxAdj;
            tmp_LineSpacingMax = target.tmp_LineSpacingMax;
            //----------------------------------------Tmp Text GradientColor----------------------------------------//
            gra_A = target.gra_A;
            gra_B = target.gra_B;
            gra_C = target.gra_C;
            gra_D = target.gra_D;
            gra_Invert = target.gra_Invert;
            gra_Used = target.gra_Used;
            gra_ColorMode = target.gra_ColorMode;
            gra_ColorModeName = target.gra_ColorModeName;
            //----------------------------------------Features----------------------------------------//
            Raycast = target.Raycast;
            Maskable = target.Maskable;
            SyncPrimitivePaintingColor = target.SyncPrimitivePaintingColor;
        }

        //------------------------通用参数设置

        /// <summary>
        /// 控制文字颜色是否受到 图元配色器 颜色控制
        /// </summary>
        /// <param name="treeState"></param>
        public void gen_Set_SyncPrimitivePaintingColor(bool state)
        {
            if (SyncPrimitivePaintingColor == state)
                return;
            SyncPrimitivePaintingColor = state;
        }

        /// <summary>
        /// 设置射线检测可用性
        /// </summary>
        /// <param name="treeState"></param>
        public void gen_RayCastSet(bool state)
        {
            if (Raycast == state)
                return;
            Raycast = state;
        }

        /// <summary>
        /// 设置蒙版可用性
        /// </summary>
        /// <param name="treeState"></param>
        public void gen_Set_MaskableSet(bool state)
        {
            if (Maskable == state)
                return;
            Maskable = state;
        }

        //------------------------Text参数设置

        /// <summary>
        /// 改变字体样式
        /// </summary>
        /// <param name="style">样式</param>
        public void txt_Set_FontStyle(FontStyle style)
        {
            if (Style == style)
                return;
            Style = style;
        }

        /// <summary>
        /// 改变文字锚点位置
        /// </summary>
        /// <param name="anchor">锚点位置_AnchoredPosition</param>
        public void txt_Set_Alignment(ContentAnchor anchor)
        {
            if (ContentAnchor == anchor)
                return;
            ContentAnchor = anchor;
        }

        /// <summary>
        /// 改变文字几何对齐
        /// </summary>
        /// <param name="treeState"></param>
        public void txt_Set_GeometreAlign(bool state)
        {
            if (GeometreAlign == state)
                return;
            GeometreAlign = state;
        }

        /// <summary>
        /// 改变字体
        /// </summary>
        /// <param name="font">字体</param>
        public void txt_Set_Font(Font font)
        {
            if (font == null)
                return;
            if (font == Font)
                return;
            Font = font;
        }

        /// <summary>
        /// 改变字体尺寸
        /// </summary>
        /// <param name="size">尺寸_Size</param>
        public void txt_Set_FontSize(float size)
        {
            if (Size == size)
                return;
            Size = size;
        }

        /// <summary>
        /// 改变字体行高
        /// </summary>
        /// <param name="height">尺寸_Size</param>
        public void txt_Set_FontLineHeight(float height)
        {
            if (LineHeight == height)
                return;
            LineHeight = height;
        }

        /// <summary>
        /// 溢出 H
        /// </summary>
        /// <param name="mode"> Overflow = 溢出, Wrap = 包裹</param>
        public void txt_Set_Overflow(HorizontalWrapMode mode)
        {
            if (Overflow_Horizon == mode)
                return;
            Overflow_Horizon = mode;
        }

        /// <summary>
        /// 溢出 V
        /// </summary>
        /// <param name="mode"> Overflow = 溢出, Truncate = 截断</param>
        public void txt_Set_Overflow(VerticalWrapMode mode)
        {
            if (Overflow_Vertical == mode)
                return;
            Overflow_Vertical = mode;
        }

        /// <summary>
        /// 设置富文本开关
        /// </summary>
        /// <param name="treeState"></param>
        public void txt_Set_RichTextEnabled(bool state)
        {
            if (RichText == state)
                return;
            RichText = state;
        }

        /// <summary>
        /// 设置自动尺寸开关
        /// </summary>
        /// <param name="treeState"></param>
        public void txt_Set_BestFit(bool state)
        {
            if (BestFit == state)
                return;
            BestFit = state;
        }

        /// <summary>
        /// 改变字体自动尺寸最小
        /// </summary>
        /// <param name="size">尺寸_Size</param>
        public void txt_Set_FontFitSize_Min(int size)
        {
            if (Fit_Min == size)
                return;
            Fit_Min = size;
        }

        /// <summary>
        /// 改变字体自动尺寸最大
        /// </summary>
        /// <param name="size">尺寸_Size</param>
        public void txt_Set_FontFitSize_Max(int size)
        {
            if (Fit_Max == size)
                return;
            Fit_Max = size;
        }

        /// <summary>
        /// 改变字体颜色
        /// </summary>
        /// <param name="color">颜色_Color</param>
        public void txt_Set_FontColor(Color color)
        {
            if (FontColor == color)
                return;
            FontColor = color;
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
            if (tmp_WrappingRatios == val)
                return;
            tmp_WrappingRatios = val;
        }

        /// <summary>
        /// 边距值设置
        /// </summary>
        /// <param name="value">边距值</param>
        public void tmp_Set_MarginSet(Vector4 value)
        {
            if (tmp_contentmargin == value)
                return;
            tmp_contentmargin = value;
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
            if (tmp_EnableAutoSizing == state)
                return;
            tmp_EnableAutoSizing = state;
        }

        /// <summary>
        /// 最佳匹配尺寸设置
        /// </summary>
        public void tmp_Set_FontAutoSize(Vector4 autosize)
        {
            if (autosize.x != tmp_FontSizeMin)
            {
                tmp_FontSizeMin = autosize.x;
            }
            if (autosize.y != tmp_FontSizeMax)
            {
                tmp_FontSizeMax = autosize.y;
            }
            if (autosize.z != tmp_CharWidthMaxAdj)
            {
                tmp_CharWidthMaxAdj = autosize.z;
            }
            if (autosize.w != tmp_LineSpacingMax)
            {
                tmp_LineSpacingMax = autosize.w;
            }
        }

        /// <summary>
        /// 最佳匹配尺寸设置 - 最小尺寸
        /// </summary>
        public void tmp_Set_FontAutoSize_FontSizeMin(float value)
        {
            if (value != tmp_FontSizeMin)
            {
                tmp_FontSizeMin = value;
            }
        }

        /// <summary>
        /// 最佳匹配尺寸设置 - 最大尺寸
        /// </summary>
        public void tmp_Set_FontAutoSize_FontSizeMax(float value)
        {
            if (value != tmp_FontSizeMax)
            {
                tmp_FontSizeMax = value;
            }
        }

        /// <summary>
        /// 最佳匹配尺寸设置 - 字符宽度百分比
        /// </summary>
        public void tmp_Set_FontAutoSize_CharWidthMaxAdj(float value)
        {
            if (value != tmp_CharWidthMaxAdj)
            {
                tmp_CharWidthMaxAdj = value;
            }
        }

        /// <summary>
        /// 最佳匹配尺寸设置 - 最大行高
        /// </summary> 
        public void tmp_Set_FontAutoSize_LineSpacingMax(float value)
        {
            if (value != tmp_LineSpacingMax)
            {
                tmp_LineSpacingMax = value;
            }
        }

        /// <summary>
        /// 设置渐变色 A
        /// </summary>
        /// <param name="col"></param>
        public void tmp_Set_GradientColor_A(Color col)
        {
            if (gra_A == col)
                return;
            gra_A = col;
        }

        /// <summary>
        /// 设置渐变色 B
        /// </summary>
        /// <param name="col"></param>
        public void tmp_Set_GradientColor_B(Color col)
        {
            if (gra_B == col)
                return;
            gra_B = col;
        }

        /// <summary>
        /// 设置渐变色 C
        /// </summary>
        /// <param name="col"></param>
        public void tmp_Set_GradientColor_C(Color col)
        {
            if (gra_C == col)
                return;
            gra_C = col;
        }

        /// <summary>
        /// 设置渐变色 D
        /// </summary>
        /// <param name="col"></param>
        public void tmp_Set_GradientColor_D(Color col)
        {
            if (gra_D == col)
                return;
            gra_D = col;
        }

        /// <summary>
        /// 设置渐变色反转
        /// </summary>
        /// <param name="treeState"></param>
        public void tmp_Set_GradientColor_Invert(bool state)
        {
            if (gra_Invert == state)
                return;
            gra_Invert = state;
        }

        /// <summary>
        /// 设置渐变色开关
        /// </summary>
        /// <param name="treeState"></param>
        public void tmp_Set_GradientColor_Enabled(bool state)
        {
            if (gra_Used == state)
                return;
            gra_Used = state;
        }

        /// <summary>
        /// 设置渐变色模式
        /// </summary>
        /// <param name="col"></param>
        public void tmp_Set_GradientMode(ColorMode mode)
        {
            if (gra_ColorMode == mode)
                return;
            gra_ColorMode = mode;
        }

        /// <summary>
        /// 设置渐变色模式名称
        /// </summary>
        /// <param name="col"></param>
        public void tmp_Set_GradientModeName(string mode)
        {
            if (gra_ColorModeName == mode)
                return;
            gra_ColorModeName = mode;
        }
    }

    [CreateAssetMenu(fileName = "XHud_Library_TextStyle", menuName = "XHud/CreateAssets (创建Hud资源库)/Library-TextStyle (文字样式库)", order = 0)]
    public class XHud_Library_TextStyle : ScriptableObject
    {
        public string LibraryName = "NewTextStyleLibrary";

        public List<XHud_LibraryArg_TextStyle> TextStyleLibrary = new List<XHud_LibraryArg_TextStyle>();

        public Font Preview_Font;
        public Color PreviewFontColor = Color.white;
        public string PreviewContent = "XHud Sample Fonts";
        public string PreviewTypeText = "Text";
        public FontStyle PreviewStyle = FontStyle.Normal;


        /// <summary>
        /// 名称项查找（精确匹配）
        /// </summary>
        public string Find = "";

        /// <summary>
        /// Editor列表项高度
        /// </summary>
        public float itemHeight = 61;

        /// <summary>
        /// 可视区域显示的元素数量
        /// </summary>
        public int visibleItemCount = 7;

        /// <summary>
        /// 列表滚动位置
        /// </summary>
        public Vector2 ColorInfoList_Original_Scroller;

        /// <summary>
        /// Editor筛选名称
        /// </summary>
        public string Highlight;

        /// <summary>
        /// 定位选中项索引号
        /// </summary>
        public int LocationSelectedIndex = -1;

        /// <summary>
        /// 选中项索引号
        /// </summary>
        public int SelectedIndex;

        public UnityAction act_on_TextStyleChanged;

        private void OnEnable()
        {
            itemHeight = 61;
            visibleItemCount = 7;
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
            return TextStyleLibrary.Count;
        }

        /// <summary>
        /// 获取字体库数量
        /// </summary>
        /// <returns></returns>
        public int TextStyle_Library_GetCount(xHud_TextType type)
        {
            int count = 0;
            for (int i = 0; i < TextStyleLibrary.Count; i++)
            {
                if (TextStyleLibrary[i].Type == type)
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
            if (TextStyleLibrary != null)
            {
                if (TextStyleLibrary.Count > 0)
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
            for (int i = 0; i < TextStyleLibrary.Count; i++)
            {
                if (name == TextStyleLibrary[i].Name)
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
            TextStyleLibrary[index].CopyData_Ignored_LibraryToggle(info);
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
            for (int i = 0; i < TextStyleLibrary.Count; i++)
            {
                if (name == TextStyleLibrary[i].Name)
                {
                    xc = i;
                    ismatched = true;
                }
            }
            if (ismatched)
                TextStyleLibrary.RemoveAt(xc);

            if (act_on_TextStyleChanged != null)
                act_on_TextStyleChanged();
        }

        /// <summary>
        /// 添加一个字体样式模版
        /// </summary>
        /// <param name="info">文字样式信息类</param>
        public void TextStyle_Library_Add(XHud_LibraryArg_TextStyle info)
        {
            TextStyleLibrary.Add(info);
            if (act_on_TextStyleChanged != null)
                act_on_TextStyleChanged();
        }

        /// <summary>
        /// 获取字体库所有样式
        /// </summary>
        /// <returns></returns>
        public XHud_LibraryArg_TextStyle[] TextStyle_Library_GetAllStyle()
        {
            return TextStyleLibrary.ToArray();
        }

        /// <summary>
        /// 获取字体库所有名称
        /// </summary>
        /// <returns></returns>
        public string[] TextStyle_Library_GetAllNames()
        {
            string[] names = new string[TextStyleLibrary.Count];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = TextStyleLibrary[i].Name;
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
            for (int i = 0; i < TextStyleLibrary.Count; i++)
            {
                if (TextStyleLibrary[i].Type == xHud_TextType.Text)
                    info.Add(TextStyleLibrary[i]);
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
            for (int i = 0; i < TextStyleLibrary.Count; i++)
            {
                if (TextStyleLibrary[i].Type == xHud_TextType.Text)
                    info.Add(TextStyleLibrary[i].Name);
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
            for (int i = 0; i < TextStyleLibrary.Count; i++)
            {
                if (TextStyleLibrary[i].Type == xHud_TextType.TmpText)
                    info.Add(TextStyleLibrary[i]);
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
            for (int i = 0; i < TextStyleLibrary.Count; i++)
            {
                if (TextStyleLibrary[i].Type == xHud_TextType.TmpText)
                    info.Add(TextStyleLibrary[i].Name);
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
            if (index < TextStyleLibrary.Count)
                return TextStyleLibrary[index];
            else
                return TextStyleLibrary[0];
        }

        /// <summary>
        /// 根据目标Index和类型获取字体样式
        /// </summary>
        /// <param name="index"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public XHud_LibraryArg_TextStyle TextStyle_Library_GetTextStyleInfo(int index, xHud_TextType type)
        {
            if (index < TextStyleLibrary.Count)
                if (TextStyleLibrary[index].Type == type)
                    return TextStyleLibrary[index];
                else
                    return null;
            else
                return TextStyleLibrary[0];
        }

        /// <summary>
        /// 获取目标名称的字体样式
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XHud_LibraryArg_TextStyle TextStyle_Library_GetTextStyleInfo(string name)
        {
            XHud_LibraryArg_TextStyle info = null;
            for (int i = 0; i < TextStyleLibrary.Count; i++)
            {
                if (TextStyleLibrary[i].Name == name)
                {
                    info = TextStyleLibrary[i];
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
            for (int i = 0; i < TextStyleLibrary.Count; i++)
            {
                if (TextStyleLibrary[i].Name == name && TextStyleLibrary[i].Type == type)
                {
                    info = TextStyleLibrary[i];
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
            for (int i = 0; i < TextStyleLibrary.Count; i++)
            {
                if (TextStyleLibrary[i].Type == type)
                    infos.Add(TextStyleLibrary[i]);
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
            for (int i = 0; i < TextStyleLibrary.Count; i++)
            {
                if (TextStyleLibrary[i].Name == name)
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
            for (int i = 0; i < TextStyleLibrary.Count; i++)
            {
                if (TextStyleLibrary[i].Name == name && TextStyleLibrary[i].Type == type)
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

            for (int i = 0; i < TextStyleLibrary.Count; i++)
            {
                if (TextStyleLibrary[i].Name == name)
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
            SelectedIndex = index;
            LocationSelectedIndex = index;
            Highlight = name;

            //计算列表滚动值
            float scrollval = itemHeight;
            for (int i = 0; i < TextStyleLibrary.Count; i++)
            {
                if (TextStyleLibrary[i].Name == name)
                {
                    if (i > visibleItemCount / 2)
                        scrollval -= (visibleItemCount / 2) * itemHeight;
                    else
                        scrollval -= itemHeight;
                    break;
                }
                else
                {
                    scrollval += itemHeight;
                }
            }
            Vector2 newscroll = ColorInfoList_Original_Scroller;
            newscroll.y = scrollval;
            ColorInfoList_Original_Scroller = newscroll;
        }

        /// <summary>
        /// 列表定位并滚动到目标 - 查找
        /// </summary>
        /// <param name="name"></param>
        public void TextStyleLibrary_Location_Find(string name)
        {
            int index = TextStyle_Library_GetIndex(name);
            SelectedIndex = index;
            LocationSelectedIndex = index;

            //计算列表滚动值
            float scrollval = itemHeight;
            for (int i = 0; i < TextStyleLibrary.Count; i++)
            {
                if (TextStyleLibrary[i].Name == name)
                {
                    if (i > visibleItemCount / 2)
                        scrollval -= (visibleItemCount / 2) * itemHeight;
                    else
                        scrollval -= itemHeight;
                    break;
                }
                else
                {
                    scrollval += itemHeight;
                }
            }
            Vector2 newscroll = ColorInfoList_Original_Scroller;
            newscroll.y = scrollval;
            ColorInfoList_Original_Scroller = newscroll;
        }
    }
}