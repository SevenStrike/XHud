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
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 重建文字模式
    /// </summary>
    public enum XHud_PSDR_TextLayerMode
    {
        文字像素化,
        文字组件化,
    }

    /// <summary>
    /// 重建文字类型
    /// </summary>
    public enum XHud_PSDR_TextLayerTypes
    {
        HudText,
        HudTmpText,
    }

    /// <summary>
    /// 重建图层类型
    /// </summary>
    public enum XHud_PSDR_LayerType
    {
        shp,
        smt,
        pix,
        txt,
        group
    }

    #region 图元数据结构类

    [System.Serializable]
    /// <summary>
    /// 日期与时间
    /// </summary>
    public class PSDR_Datetime
    {
        /// <summary>
        /// 日期
        /// </summary>
        public string date;

        /// <summary>
        /// 时间
        /// </summary>
        public string time;

    }

    [System.Serializable]
    /// <summary>
    /// 尺寸_Size
    /// </summary>
    public class PSDR_Size
    {
        /// <summary>
        /// 宽度
        /// </summary>
        public int width;

        /// <summary>
        /// 高度
        /// </summary>
        public int height;
    }

    [System.Serializable]
    /// <summary>
    /// 位置_Position
    /// </summary>
    public class PSDR_Position
    {
        /// <summary>
        /// 水平位置
        /// </summary>
        public int x;

        /// <summary>
        /// 垂直位置
        /// </summary>
        public int y;

        /// <summary>
        /// 无效果水平位置
        /// </summary>
        public int x_bypass;

        /// <summary>
        /// 无效果垂直位置
        /// </summary>
        public int y_bypass;

    }

    [System.Serializable]
    /// <summary>
    /// 父物体
    /// </summary>
    public class PSDR_Parent
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name;

        /// <summary>
        /// ID
        /// </summary>
        public int id;

    }

    [System.Serializable]
    /// <summary>
    /// 颜色_Color - RGB
    /// </summary>
    public class PSDR_Color_rgb
    {
        /// <summary>
        /// 颜色通道 - R
        /// </summary>
        public float r;

        /// <summary>
        /// 颜色通道 - G
        /// </summary>
        public float g;

        /// <summary>
        /// 颜色通道 - B
        /// </summary>
        public float b;
    }

    [System.Serializable]
    /// <summary>
    /// 颜色_Color - RGBA
    /// </summary>
    public class PSDR_Color_rgba
    {
        /// <summary>
        /// 颜色通道 - R
        /// </summary>
        public float r;

        /// <summary>
        /// 颜色通道 - G
        /// </summary>
        public float g;

        /// <summary>
        /// 颜色通道 - B
        /// </summary>
        public float b;

        /// <summary>
        /// 颜色通道 - A
        /// </summary>
        public float a;
    }

    [System.Serializable]
    /// <summary>
    /// 颜色_Color - HSV
    /// </summary>
    public class PSDR_Color_hsv
    {
        /// <summary>
        /// 颜色通道 - 色调
        /// </summary>
        public float h;

        /// <summary>
        /// 颜色通道 - 饱和度
        /// </summary>
        public float s;

        /// <summary>
        /// 颜色通道 - 明度
        /// </summary>
        public float v;
    }

    [System.Serializable]
    /// <summary>
    /// 颜色_Color - HSVA
    /// </summary>
    public class PSDR_Color_hsva
    {
        /// <summary>
        /// 颜色通道 - 色调
        /// </summary>
        public float h;

        /// <summary>
        /// 颜色通道 - 饱和度
        /// </summary>
        public float s;

        /// <summary>
        /// 颜色通道 - 明度
        /// </summary>
        public float v;

        /// <summary>
        /// 颜色通道 - 透明度_Alpha
        /// </summary>
        public float a;
    }

    [System.Serializable]
    /// <summary>
    /// 颜色_Color - RGB (0-1)
    /// </summary>
    public class PSDR_Color_clamp_rgb
    {
        /// <summary>
        /// 颜色通道 - 限制在0-1范围 - R
        /// </summary>
        public float r;

        /// <summary>
        /// 颜色通道 - 限制在0-1范围 - G
        /// </summary>
        public float g;

        /// <summary>
        /// 颜色通道 - 限制在0-1范围 - B
        /// </summary>
        public float b;
    }

    [System.Serializable]
    /// <summary>
    /// 颜色_Color - RGBA (0-1)
    /// </summary>
    public class PSDR_Color_clamp_rgba
    {
        /// <summary>
        /// 颜色通道 - 限制在0-1范围 - R
        /// </summary>
        public float r;

        /// <summary>
        /// 颜色通道 - 限制在0-1范围 - G
        /// </summary>
        public float g;

        /// <summary>
        /// 颜色通道 - 限制在0-1范围 - B
        /// </summary>
        public float b;

        /// <summary>
        /// 颜色通道 - 限制在0-1范围 - A
        /// </summary>
        public float a;
    }

    [System.Serializable]
    /// <summary>
    /// 字体-颜色_Color-RGB
    /// </summary>
    public class PSDR_Ft_color_rgb
    {
        /// <summary>
        /// 字体 - 颜色_Color - R
        /// </summary>
        public float r;

        /// <summary>
        /// 字体 - 颜色_Color - G
        /// </summary>
        public float g;

        /// <summary>
        /// 字体 - 颜色_Color - B
        /// </summary>
        public float b;
    }

    [System.Serializable]
    /// <summary>
    /// 字体-颜色_Color-RGBA
    /// </summary>
    public class PSDR_Ft_color_rgba
    {
        /// <summary>
        /// 字体 - 颜色_Color - R
        /// </summary>
        public float r;

        /// <summary>
        /// 字体 - 颜色_Color - G
        /// </summary>
        public float g;

        /// <summary>
        /// 字体 - 颜色_Color - B
        /// </summary>
        public float b;

        /// <summary>
        /// 字体 - 颜色_Color - A
        /// </summary>
        public float a;
    }

    [System.Serializable]
    /// <summary>
    /// 字体-颜色_Color-HSV
    /// </summary>
    public class PSDR_Ft_color_hsv
    {
        /// <summary>
        /// 字体 - 颜色_Color - 色调
        /// </summary>
        public float h;

        /// <summary>
        /// 字体 - 颜色_Color - 饱和度
        /// </summary>
        public float s;

        /// <summary>
        /// 字体 - 颜色_Color - 明度
        /// </summary>
        public float v;
    }

    [System.Serializable]
    /// <summary>
    /// 字体-颜色_Color-HSVA
    /// </summary>
    public class PSDR_Ft_color_hsva
    {
        /// <summary>
        /// 字体 - 颜色_Color - 色调
        /// </summary>
        public float h;

        /// <summary>
        /// 字体 - 颜色_Color - 饱和度
        /// </summary>
        public float s;

        /// <summary>
        /// 字体 - 颜色_Color - 明度
        /// </summary>
        public float v;

        /// <summary>
        /// 字体 - 颜色_Color - 透明度_Alpha
        /// </summary>
        public float a;
    }

    [System.Serializable]
    /// <summary>
    /// 字体-颜色_Color-RGB (0-1)
    /// </summary>
    public class PSDR_Ft_color_clamp_rgb
    {
        /// <summary>
        /// 字体 - 颜色_Color - 限制0-1范围 - R
        /// </summary>
        public float r;

        /// <summary>
        /// 字体 - 颜色_Color - 限制0-1范围 - G
        /// </summary>
        public float g;

        /// <summary>
        /// 字体 - 颜色_Color - 限制0-1范围 - B
        /// </summary>
        public float b;
    }

    [System.Serializable]
    /// <summary>
    /// 字体-颜色_Color-RGBA (0-1)
    /// </summary>
    public class PSDR_Ft_color_clamp_rgba
    {
        /// <summary>
        /// 字体 - 颜色_Color - 限制0-1范围 - R
        /// </summary>
        public float r;

        /// <summary>
        /// 字体 - 颜色_Color - 限制0-1范围 - G
        /// </summary>
        public float g;

        /// <summary>
        /// 字体 - 颜色_Color - 限制0-1范围 - B
        /// </summary>
        public float b;

        /// <summary>
        /// 字体 - 颜色_Color - 限制0-1范围 - A
        /// </summary>
        public float a;
    }

    [System.Serializable]
    /// <summary>
    /// 图层属性
    /// </summary>
    public class PSDR_Layers
    {
        /// <summary>
        /// 图层绝对路径
        /// </summary>
        public string Path;

        /// <summary>
        /// 图层ID
        /// </summary>
        public int id;

        /// <summary>
        /// 图层名称
        /// </summary>
        public string name;

        /// <summary>
        /// 图层类型
        /// </summary>
        public XHud_PSDR_LayerType type;

        /// <summary>
        /// 是否为Mask遮罩图层
        /// </summary>
        public bool mask;

        /// <summary>
        /// 图层尺寸
        /// </summary>
        public PSDR_Size size;

        /// <summary>
        /// 图层位置
        /// </summary>
        public PSDR_Position position;

        /// <summary>
        /// 图层父物体
        /// </summary>
        public PSDR_Parent parent;

        /// <summary>
        /// 图层透明度
        /// </summary>
        public float opacity;

        /// <summary>
        /// 图层颜色 - RGB
        /// </summary>
        public PSDR_Color_rgb color_rgb;

        /// <summary>
        /// 图层颜色 - RGBA
        /// </summary>
        public PSDR_Color_rgba color_rgba;

        /// <summary>
        /// 图层颜色 - HSV
        /// </summary>
        public PSDR_Color_hsv color_hsv;

        /// <summary>
        /// 图层颜色 - HSVA
        /// </summary>
        public PSDR_Color_hsva color_hsva;

        /// <summary>
        /// 图层颜色 - 限制0-1范围 - RGB
        /// </summary>
        public PSDR_Color_clamp_rgb color_clamp_rgb;

        /// <summary>
        /// 图层颜色 - 限制0-1范围 - RGBA
        /// </summary>
        public PSDR_Color_clamp_rgba color_clamp_rgba;

        /// <summary>
        /// 图层颜色 - 16进制字符串
        /// </summary>
        public string color_hex;

        /// <summary>
        /// 字体图层 - 内容
        /// </summary>
        public string text;

        /// <summary>
        /// 字体图层 - 字体名称
        /// </summary>
        public string ft_name;

        /// <summary>
        /// 字体图层 - 字体家族
        /// </summary>
        public string ft_family;

        /// <summary>
        /// 字体图层 - 字体样式
        /// </summary>
        public string ft_style;

        /// <summary>
        /// 字体图层 - 字体尺寸
        /// </summary>
        public float ft_size;

        /// <summary>
        /// 字体图层 - 字体对齐
        /// </summary>
        public string ft_align;

        /// <summary>
        /// 字体图层 - 字体间距
        /// </summary>
        public float ft_space;

        /// <summary>
        /// 字体图层 - 字体是否是文本框模式
        /// </summary>
        public bool ft_isWrapText;

        /// <summary>
        /// 字体图层 - 字体大写标记
        /// </summary>
        public bool ft_isuppercase;

        /// <summary>
        /// 字体图层 - 字体行高
        /// </summary>
        public float ft_leading;

        /// <summary>
        /// 字体图层 - 颜色_Color - RGB
        /// </summary>
        public PSDR_Ft_color_rgb ft_color_rgb;

        /// <summary>
        /// 字体图层 - 颜色_Color - RGBA
        /// </summary>
        public PSDR_Ft_color_rgba ft_color_rgba;

        /// <summary>
        /// 字体图层 - 颜色_Color - HSV
        /// </summary>
        public PSDR_Ft_color_hsv ft_color_hsv;

        /// <summary>
        /// 字体图层 - 颜色_Color - HSVA
        /// </summary>
        public PSDR_Ft_color_hsva ft_color_hsva;

        /// <summary>
        /// 字体图层 - 颜色_Color - 限制0-1范围 - RGB
        /// </summary>
        public PSDR_Ft_color_clamp_rgb ft_color_clamp_rgb;

        /// <summary>
        /// 字体图层 - 颜色_Color - 限制0-1范围 - RGBA
        /// </summary>
        public PSDR_Ft_color_clamp_rgba ft_color_clamp_rgba;

        /// <summary>
        /// 字体图层 - 颜色_Color - 16进制字符串
        /// </summary>
        public string ft_color_hex;
    }

    [System.Serializable]
    /// <summary>
    /// 分组属性
    /// </summary>
    public class PSDR_Groups
    {
        /// <summary>
        /// 组名称
        /// </summary>
        public string name;

        /// <summary>
        /// 组ID
        /// </summary>
        public int id;

        /// <summary>
        /// 父物体
        /// </summary>
        public PSDR_Parent parent;

        /// <summary>
        /// 子图层数量
        /// </summary>
        public int childs;

    }

    [System.Serializable]
    /// <summary>
    /// PSD的基础与所有结构信息
    /// </summary>
    public class PSDR_Structure
    {
        [SerializeField]
        /// <summary>
        /// PSD - 名称
        /// </summary>
        public string name;
        [SerializeField]
        /// <summary>
        /// PSD - 颜色模式
        /// </summary>
        public string colormode;
        [SerializeField]
        /// <summary>
        /// PSD - 导出日期&时间
        /// </summary>
        public PSDR_Datetime datetime;

        /// <summary>
        /// PSD - 画布尺寸
        /// </summary>
        public string size;

        /// <summary>
        /// PSD - 分辨率
        /// </summary>
        public int dpi;

        /// <summary>
        /// PSD - 绝对路径
        /// </summary>
        public string path_psd;

        /// <summary>
        /// PSD - 图集文件夹的绝对路径
        /// </summary>
        public string path_sprites;

        /// <summary>
        /// PSD - 总图层数量
        /// </summary>
        public int count_layer;

        /// <summary>
        /// PSD - 总编组数量
        /// </summary>
        public int count_group;

        /// <summary>
        /// PSD - 图层数据集合
        /// </summary>
        public List<PSDR_Layers> layers;

        /// <summary>
        /// PSD - 编组数据集合
        /// </summary>
        public List<PSDR_Groups> groups;
    }

    [System.Serializable]
    /// <summary>
    /// PSD - 数据根节点
    /// </summary>
    public class PSDR_Root
    {
        [SerializeField]
        /// <summary>
        /// PSD - 数据结构
        /// </summary>
        public PSDR_Structure structure;
    }
    #endregion
}