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
    using System;
    using System.IO;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.Callbacks;
#endif
    using UnityEngine;
    using System.Collections.Generic;
    using SevenStrikeModules.XHud.Enums;

    #region 配置类
    /// <summary>
    /// 颜色预设的轻量值类型，用于在创建默认配置时临时承载“名称 + 颜色”键值对
    /// </summary>
    /// <remarks>
    /// 该结构体仅在 <see cref="XHud_Dashboard.config_create"/> 内部作为临时容器使用，
    /// 最终会被转换为可序列化的 <see cref="Config_Color_Preset"/>
    /// </remarks>
    public struct colorpreset
    {
        /// <summary>
        /// 预设颜色的显示名称
        /// </summary>
        public string name;
        /// <summary>
        /// 预设颜色值
        /// </summary>
        public Color color;

        /// <summary>
        /// 使用名称与颜色初始化一个 <see cref="colorpreset"/> 实例
        /// </summary>
        /// <param name="name">预设颜色的显示名称</param>
        /// <param name="color">预设颜色值</param>
        public colorpreset(string name, Color color)
        {
            this.name = name;
            this.color = color;
        }
    }
    /// <summary>
    /// XHud 版本与授权信息的值类型容器
    /// </summary>
    /// <remarks>
    /// 由 <see cref="XHud_Dashboard.get_xhud_info"/> 从当前配置中提取并返回给调用方
    /// </remarks>
    public struct versioninfo
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string version;
        /// <summary>
        /// 授权协议名称
        /// </summary>
        public string licence;
        /// <summary>
        /// 副标题/子名称
        /// </summary>
        public string sub;
        /// <summary>
        /// 插件简介
        /// </summary>
        public string intro;
        /// <summary>
        /// 官方网站地址
        /// </summary>
        public string website;
        /// <summary>
        /// 在线文档地址
        /// </summary>
        public string document;
        /// <summary>
        /// 技术支持地址
        /// </summary>
        public string support;

        /// <summary>
        /// 使用完整字段初始化一个 <see cref="versioninfo"/> 实例
        /// </summary>
        /// <param name="version">版本号</param>
        /// <param name="licence">授权协议名称</param>
        /// <param name="sub">副标题/子名称</param>
        /// <param name="intro">插件简介</param>
        /// <param name="website">官方网站地址</param>
        /// <param name="document">在线文档地址</param>
        /// <param name="support">技术支持地址</param>
        public versioninfo(string version, string licence, string sub, string intro, string website, string document, string support)
        {
            this.version = version;
            this.licence = licence;
            this.sub = sub;
            this.intro = intro;
            this.website = website;
            this.document = document;
            this.support = support;
        }
    }
    /// <summary>
    /// 工具类功能的配置数据（可序列化）
    /// </summary>
    /// <remarks>
    /// 对应仪表盘“工具”分类下的开关项，例如屏幕准心瞄具、拖拽放置元素等
    /// </remarks>
    [Serializable]
    public class Config_Utilitys
    {
        /// <summary>是否显示屏幕准心瞄具</summary>
        public bool util_screeen_aimer;
        /// <summary>是否启用“元素通过拖拽放置到屏幕锚点”的可视化放置工具</summary>
        public bool util_screeen_element_visual_placer;

        /// <summary>创建默认（全部关闭）的工具配置实例</summary>
        public Config_Utilitys() { }

        /// <summary>
        /// 使用指定开关状态初始化工具配置
        /// </summary>
        /// <param name="util_screeen_aimer">是否显示屏幕准心瞄具</param>
        /// <param name="util_screeen_element_visual_placer">是否启用可视化放置工具</param>
        public Config_Utilitys(bool util_screeen_aimer, bool util_screeen_element_visual_placer)
        {
            this.util_screeen_aimer = util_screeen_aimer;
            this.util_screeen_element_visual_placer = util_screeen_element_visual_placer;
        }

        /// <summary>
        /// 创建当前实例的深拷贝
        /// </summary>
        /// <returns>字段值完全一致的新实例</returns>
        public Config_Utilitys Clone()
        {
            return new Config_Utilitys(util_screeen_aimer, util_screeen_element_visual_placer);
        }

        /// <summary>
        /// 从另一个实例复制字段值到当前实例
        /// </summary>
        /// <param name="other">数据来源实例；为 <c>null</c> 时直接返回</param>
        public void CopyFrom(Config_Utilitys other)
        {
            if (other == null) return;
            util_screeen_aimer = other.util_screeen_aimer;
            util_screeen_element_visual_placer = other.util_screeen_element_visual_placer;
        }

        /// <summary>
        /// 将全部字段重置为默认值（<c>false</c>）
        /// </summary>
        public void Reset()
        {
            util_screeen_aimer = false;
            util_screeen_element_visual_placer = false;
        }
    }
    /// <summary>
    /// 单个颜色预设项（可序列化），由名称与颜色组成
    /// </summary>
    [Serializable]
    public class Config_Color_Preset
    {
        /// <summary>
        /// 预设颜色的显示名称
        /// </summary>
        public string name;
        /// <summary>
        /// 预设颜色值
        /// </summary>
        public Color color;

        /// <summary>
        /// 创建默认（空名称、默认颜色）的预设项
        /// </summary>
        public Config_Color_Preset() { }

        /// <summary>
        /// 使用颜色与名称初始化预设项
        /// </summary>
        /// <param name="color">预设颜色值</param>
        /// <param name="name">预设颜色的显示名称</param>
        public Config_Color_Preset(Color color, string name)
        {
            this.name = name;
            this.color = color;
        }

        /// <summary>
        /// 创建当前实例的拷贝（当前字段均为值类型，等价于值复制）
        /// </summary>
        /// <returns>字段值完全一致的新实例</returns>
        public Config_Color_Preset Clone()
        {
            return new Config_Color_Preset(color, name);
        }

        /// <summary>
        /// 从另一个实例复制字段值到当前实例
        /// </summary>
        /// <param name="other">数据来源实例；为 <c>null</c> 时直接返回</param>
        public void CopyFrom(Config_Color_Preset other)
        {
            if (other == null) return;
            color = other.color;
            name = other.name;
        }

        /// <summary>
        /// 将字段重置为默认值（默认颜色与空名称）
        /// </summary>
        public void Reset()
        {
            color = default;
            name = string.Empty;
        }
    }
    /// <summary>
    /// 主题配置数据（可序列化），包含主色调、编组色、分割线色及其预设列表
    /// </summary>
    [Serializable]
    public class Config_Themes
    {
        /// <summary>
        /// 主题主色调
        /// </summary>
        public Color primary;
        /// <summary>
        /// 主题编组（面板）颜色
        /// </summary>
        public Color group;
        /// <summary>
        /// 主题分割线颜色
        /// </summary>
        public Color sepline;

        /// <summary>
        /// 主色调的预设颜色列表
        /// </summary>
        public List<Config_Color_Preset> presets_primary;
        /// <summary>
        /// 编组色的预设颜色列表
        /// </summary>
        public List<Config_Color_Preset> presets_group;
        /// <summary>
        /// 分割线色的预设颜色列表
        /// </summary>
        public List<Config_Color_Preset> presets_seperateline;

        /// <summary>
        /// 创建空主题配置，所有预设列表初始化为空列表（非 <c>null</c>）
        /// </summary>
        public Config_Themes()
        {
            presets_primary = new List<Config_Color_Preset>();
            presets_group = new List<Config_Color_Preset>();
            presets_seperateline = new List<Config_Color_Preset>();
        }

        /// <summary>
        /// 使用完整字段初始化主题配置，并对传入的预设列表做逐项深拷贝
        /// </summary>
        /// <param name="primary">主题主色调</param>
        /// <param name="group">主题编组颜色</param>
        /// <param name="seperateline">主题分割线颜色</param>
        /// <param name="presets_primary">主色调预设列表，可为 <c>null</c></param>
        /// <param name="presets_group">编组色预设列表，可为 <c>null</c></param>
        /// <param name="presets_seperateline">分割线色预设列表，可为 <c>null</c></param>
        public Config_Themes(Color primary, Color group, Color seperateline, List<Config_Color_Preset> presets_primary, List<Config_Color_Preset> presets_group, List<Config_Color_Preset> presets_seperateline)
        {
            this.primary = primary;
            this.group = group;
            this.sepline = seperateline;

            this.presets_primary = new List<Config_Color_Preset>();
            if (presets_primary != null)
                foreach (var item in presets_primary)
                    this.presets_primary.Add(item.Clone());

            this.presets_group = new List<Config_Color_Preset>();
            if (presets_group != null)
                foreach (var item in presets_group)
                    this.presets_group.Add(item.Clone());

            this.presets_seperateline = new List<Config_Color_Preset>();
            if (presets_seperateline != null)
                foreach (var item in presets_seperateline)
                    this.presets_seperateline.Add(item.Clone());
        }

        /// <summary>
        /// 创建当前实例的深拷贝（预设列表逐项拷贝）
        /// </summary>
        /// <returns>字段值完全一致的新实例</returns>
        public Config_Themes Clone()
        {
            return new Config_Themes(primary, group, sepline, presets_primary, presets_group, presets_seperateline);
        }

        /// <summary>
        /// 从另一个实例复制字段值到当前实例（预设列表逐项深拷贝）
        /// </summary>
        /// <param name="other">数据来源实例；为 <c>null</c> 时直接返回</param>
        public void CopyFrom(Config_Themes other)
        {
            if (other == null) return;
            primary = other.primary;
            group = other.group;
            sepline = other.sepline;

            this.presets_primary = new List<Config_Color_Preset>();
            if (presets_primary != null)
                foreach (var item in presets_primary)
                    this.presets_primary.Add(item.Clone());

            this.presets_group = new List<Config_Color_Preset>();
            if (presets_group != null)
                foreach (var item in presets_group)
                    this.presets_group.Add(item.Clone());

            this.presets_seperateline = new List<Config_Color_Preset>();
            if (presets_seperateline != null)
                foreach (var item in presets_seperateline)
                    this.presets_seperateline.Add(item.Clone());
        }

        /// <summary>
        /// 将颜色字段重置为默认值，并清空所有预设列表
        /// </summary>
        public void Reset()
        {
            primary = default;
            group = default;
            sepline = default;

            presets_primary = new List<Config_Color_Preset>();
            presets_seperateline = new List<Config_Color_Preset>();
            presets_group = new List<Config_Color_Preset>();
        }
    }
    /// <summary>
    /// 图元（Primitive）相关的配置数据（可序列化）
    /// </summary>
    [Serializable]
    public class Config_Primitives
    {
        /// <summary>
        /// 是否在场景中实时显示图元 ID
        /// </summary>
        public bool controller_id_displayer;

        /// <summary>
        /// 创建默认（关闭）的图元配置实例
        /// </summary>
        public Config_Primitives() { }

        /// <summary>
        /// 使用指定开关状态初始化图元配置
        /// </summary>
        /// <param name="controllerIdDisplayer">是否在场景中实时显示图元 ID</param>
        public Config_Primitives(bool controllerIdDisplayer)
        {
            controller_id_displayer = controllerIdDisplayer;
        }

        /// <summary>
        /// 创建当前实例的深拷贝
        /// </summary>
        /// <returns>字段值完全一致的新实例</returns>
        public Config_Primitives Clone()
        {
            return new Config_Primitives(controller_id_displayer);
        }

        /// <summary>
        /// 从另一个实例复制字段值到当前实例
        /// </summary>
        /// <param name="other">数据来源实例；为 <c>null</c> 时直接返回</param>
        public void CopyFrom(Config_Primitives other)
        {
            if (other == null) return;
            controller_id_displayer = other.controller_id_displayer;
        }

        /// <summary>
        /// 将字段重置为默认值（<c>false</c>）
        /// </summary>
        public void Reset()
        {
            controller_id_displayer = false;
        }
    }
    /// <summary>
    /// 元素（Element）相关的配置数据（可序列化），主要控制动画预览行为
    /// </summary>
    [Serializable]
    public class Config_Elements
    {
        /// <summary>
        /// 预览选项 - 自动杀死预览动画
        /// </summary>
        public bool previewtweens_autokill;
        /// <summary>
        /// 预览选项 - 杀死后自动倒退（重置）动画
        /// </summary>
        public bool previewtweens_rewindwithkill;
        /// <summary>
        /// 预览选项 - 杀死后清除预览列表
        /// </summary>
        public bool previewtweens_clearwithkill;

        /// <summary>
        /// 创建默认（全部关闭）的元素配置实例
        /// </summary>
        public Config_Elements() { }

        /// <summary>
        /// 使用指定开关状态初始化元素配置
        /// </summary>
        /// <param name="autokill">是否自动杀死预览动画</param>
        /// <param name="rewindWithKill">杀死后是否自动倒退动画</param>
        /// <param name="clearWithKill">杀死后是否清除预览列表</param>
        public Config_Elements(bool autokill, bool rewindWithKill, bool clearWithKill)
        {
            previewtweens_autokill = autokill;
            previewtweens_rewindwithkill = rewindWithKill;
            previewtweens_clearwithkill = clearWithKill;
        }

        /// <summary>
        /// 创建当前实例的深拷贝
        /// </summary>
        /// <returns>字段值完全一致的新实例</returns>
        public Config_Elements Clone()
        {
            return new Config_Elements(
                previewtweens_autokill,
                previewtweens_rewindwithkill,
                previewtweens_clearwithkill);
        }

        /// <summary>
        /// 从另一个实例复制字段值到当前实例
        /// </summary>
        /// <param name="other">数据来源实例；为 <c>null</c> 时直接返回</param>
        public void CopyFrom(Config_Elements other)
        {
            if (other == null) return;
            previewtweens_autokill = other.previewtweens_autokill;
            previewtweens_rewindwithkill = other.previewtweens_rewindwithkill;
            previewtweens_clearwithkill = other.previewtweens_clearwithkill;
        }

        /// <summary>
        /// 将全部字段重置为默认值（<c>false</c>）
        /// </summary>
        public void Reset()
        {
            previewtweens_autokill = false;
            previewtweens_rewindwithkill = false;
            previewtweens_clearwithkill = false;
        }
    }
    /// <summary>
    /// XHud 插件的根配置数据（可序列化），聚合版本信息与各分类配置
    /// </summary>
    /// <remarks>
    /// 该类型会通过 <see cref="JsonUtility"/> 序列化为 <c>XHudConfigs.json</c>，
    /// 由 <see cref="XHud_Dashboard.config_load"/> / <see cref="XHud_Dashboard.config_save"/> 读写
    /// </remarks>
    [Serializable]
    public class XHudConfigs
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string version;
        /// <summary>
        /// 授权协议名称
        /// </summary>
        public string licence;
        /// <summary>
        /// 副标题/子名称
        /// </summary>
        public string sub;
        /// <summary>
        /// 技术支持地址
        /// </summary>
        public string support;
        /// <summary>
        /// 插件简介
        /// </summary>
        public string intro;
        /// <summary>
        /// 官方网站地址
        /// </summary>
        public string website;
        /// <summary>
        /// 在线文档地址
        /// </summary>
        public string document;

        /// <summary>工具类配置
        /// </summary>
        public Config_Utilitys utilitys;
        /// <summary>
        /// 元素类配置
        /// </summary>
        public Config_Elements elements;
        /// <summary>
        /// 图元类配置
        /// </summary>
        public Config_Primitives primitives;
        /// <summary>
        /// 主题类配置
        /// </summary>
        public Config_Themes themes;

        /// <summary>
        /// 创建空配置，字符串字段置为空串，各分类配置字段初始化为非 <c>null</c> 实例
        /// </summary>
        public XHudConfigs()
        {
            licence = string.Empty;
            version = string.Empty;
            sub = string.Empty;
            support = string.Empty;
            intro = string.Empty;
            website = string.Empty;
            document = string.Empty;

            // 实例初始化：确保所有引用字段不为 null
            utilitys = new Config_Utilitys();
            elements = new Config_Elements();
            primitives = new Config_Primitives();
            themes = new Config_Themes();
        }

        /// <summary>
        /// 使用完整字段初始化根配置
        /// </summary>
        /// <param name="version">版本号</param>
        /// <param name="licence">授权协议名称</param>
        /// <param name="sub">副标题/子名称</param>
        /// <param name="support">技术支持地址</param>
        /// <param name="intro">插件简介</param>
        /// <param name="website">官方网站地址</param>
        /// <param name="document">在线文档地址</param>
        /// <param name="utilitys">工具类配置；为 <c>null</c> 时创建默认实例</param>
        /// <param name="elements">元素类配置；为 <c>null</c> 时创建默认实例</param>
        /// <param name="primitives">图元类配置；为 <c>null</c> 时创建默认实例</param>
        /// <param name="themes">主题类配置；为 <c>null</c> 时创建默认实例</param>
        public XHudConfigs(string version, string licence, string sub, string support, string intro, string website, string document, Config_Utilitys utilitys, Config_Elements elements, Config_Primitives primitives, Config_Themes themes)
        {
            this.version = version ?? string.Empty;
            this.licence = licence ?? string.Empty;
            this.sub = sub ?? string.Empty;
            this.support = support ?? string.Empty;
            this.intro = intro ?? string.Empty;
            this.website = website ?? string.Empty;
            this.document = document ?? string.Empty;

            this.utilitys = utilitys ?? new Config_Utilitys();
            this.elements = elements ?? new Config_Elements();
            this.primitives = primitives ?? new Config_Primitives();
            this.themes = themes ?? new Config_Themes();
        }

        /// <summary>
        /// 创建当前根配置的深拷贝
        /// </summary>
        /// <returns>字段值与子配置均被深拷贝的新实例</returns>
        public XHudConfigs Clone()
        {
            return new XHudConfigs(
                version,
                licence,
                sub,
                support,
                intro,
                website,
                document,
                utilitys?.Clone(),
                elements?.Clone(),
                primitives?.Clone(),
                themes?.Clone());
        }

        /// <summary>
        /// 从另一个实例复制字段值到当前实例（引用字段执行深拷贝）
        /// </summary>
        /// <param name="other">数据来源实例；为 <c>null</c> 时直接返回</param>
        public void CopyFrom(XHudConfigs other)
        {
            if (other == null) return;

            version = other.version;
            licence = other.licence;
            sub = other.sub;
            support = other.support;
            intro = other.intro;
            website = other.website;
            document = other.document;

            if (other.utilitys == null)
                utilitys = new Config_Utilitys();
            else
                utilitys.CopyFrom(other.utilitys);

            if (other.elements == null)
                elements = new Config_Elements();
            else
                elements.CopyFrom(other.elements);

            if (other.primitives == null)
                primitives = new Config_Primitives();
            else
                primitives.CopyFrom(other.primitives);

            if (other.themes == null)
                themes = new Config_Themes();
            else
                themes.CopyFrom(other.themes);
        }

        /// <summary>
        /// 将根配置及全部子配置重置为默认值
        /// </summary>
        public void Reset()
        {
            version = string.Empty;
            licence = string.Empty;
            sub = string.Empty;
            support = string.Empty;
            intro = string.Empty;
            website = string.Empty;
            document = string.Empty;

            utilitys ??= new Config_Utilitys();
            utilitys.Reset();

            elements ??= new Config_Elements();
            elements.Reset();

            primitives ??= new Config_Primitives();
            primitives.Reset();

            themes ??= new Config_Themes();
            themes.Reset();
        }
    }
    #endregion

    /// <summary>
    /// XHud 插件仪表盘静态入口，集中提供配置读取/保存、主题色、路径与运行期工具访问
    /// </summary>
    /// <remarks>
    /// <para>配置数据保存在 <see cref="Configs"/> 字段中，并通过 <c>XHudConfigs.json</c> 持久化</para>
    /// <para>本类同时暴露若干事件回调（<see cref="on_config_create"/> 等），供编辑器扩展监听配置生命周期</para>
    /// </remarks>
    public static class XHud_Dashboard
    {
        /// <summary>
        /// 当前生效的 XHud 根配置
        /// </summary>
        public static XHudConfigs Configs = new XHudConfigs();

        /// <summary>
        /// 配置被创建时触发，参数为新建的配置实例
        /// </summary>
        public static Action<XHudConfigs> on_config_create;
        /// <summary>
        /// 配置从文件载入后触发，参数为载入的配置实例
        /// </summary>
        public static Action<XHudConfigs> on_config_load;
        /// <summary>
        /// 配置保存到文件后触发，参数为已保存的配置实例
        /// </summary>
        public static Action<XHudConfigs> on_config_save;
        /// <summary>
        /// 配置发生变化时触发，参数为变更后的配置实例
        /// </summary>
        public static Action<XHudConfigs> on_config_changed;

        #region 获取 XHudManger
        /// <summary>
        /// 编辑器（非播放模式）下缓存的 <see cref="XHud_Manager"/> 引用
        /// </summary>
        public static XHud_Manager manager;
        /// <summary>
        /// 获取当前场景中的 <see cref="XHud_Manager"/> 实例
        /// </summary>
        /// <returns>
        /// 播放模式下返回 <see cref="XHud_Manager.instance"/>；
        /// 编辑模式下返回场景中首个激活的 <see cref="XHud_Manager"/>（并缓存到 <see cref="manager"/>）
        /// </returns>
        public static XHud_Manager get_manager()
        {
            if (Application.isPlaying)
                return XHud_Manager.instance;
            else
            {
                if (manager == null)
                    manager = Transform.FindFirstObjectByType<XHud_Manager>(FindObjectsInactive.Exclude);
                return manager;
            }
        }
        #endregion

        #region 获取XHud信息
        /// <summary>
        /// 从当前配置中提取并返回 XHud 的版本与授权信息
        /// </summary>
        /// <returns>填充了版本、授权、简介、官网、文档与支持地址的 <see cref="versioninfo"/> 实例</returns>
        public static versioninfo get_xhud_info()
        {
            versioninfo info = new versioninfo();
            info.version = Configs.version;
            info.licence = Configs.licence;
            info.sub = Configs.sub;
            info.intro = Configs.intro;
            info.website = Configs.website;
            info.document = Configs.document;
            info.support = Configs.support;
            return info;
        }
        #endregion

        #region 通用类配置 - 主题色
        /// <summary>
        /// 将主题色（主色调、编组色、分割线色）恢复为默认值并保存配置
        /// </summary>
        public static void themes_reset_color()
        {
            Configs.themes.primary = XGUI_Utilitys.HexString_To_Color("#3BFE9B");
            Configs.themes.group = XGUI_Utilitys.HexString_To_Color("#1E1E1E");
            Configs.themes.sepline = XGUI_Utilitys.HexString_To_Color("#535353");

            XGUI_Utilitys.Console("XHud 仪表盘消息", $"已将主题颜色重置到默认状态！", XGUIMsgState.确认);

            config_save();
        }
        /// <summary>
        /// 按主题颜色类型读取当前颜色
        /// </summary>
        /// <param name="type">主题颜色类型（主色调 / 编组 / 分割线）</param>
        /// <returns>对应颜色；配置缺失时返回回退色（主色调回退为白色，编组回退为黑色，分割线回退为灰色）</returns>
        public static Color themes_get_color(ThemeColorType type)
        {
            switch (type)
            {
                case ThemeColorType.primary:
                    return Configs?.themes?.primary ?? Color.white;
                case ThemeColorType.group:
                    return Configs?.themes?.group ?? Color.black;
                case ThemeColorType.sepline:
                    return Configs?.themes?.sepline ?? Color.gray;
                default:
                    return Color.white;
            }
        }
        /// <summary>
        /// 按主题颜色类型写入颜色并保存配置
        /// </summary>
        /// <param name="type">主题颜色类型（主色调 / 编组 / 分割线）</param>
        /// <param name="color">要写入的颜色值</param>
        public static void themes_set_color(ThemeColorType type, Color color)
        {
            switch (type)
            {
                case ThemeColorType.primary:
                    Configs.themes.primary = color;
                    XGUI_Utilitys.Console("XHud 仪表盘消息", $"已将主题主色调颜色设为：{color}！", XGUIMsgState.确认);
                    break;
                case ThemeColorType.group:
                    Configs.themes.group = color;
                    XGUI_Utilitys.Console("XHud 仪表盘消息", $"已将主题编组颜色设为：{color}！", XGUIMsgState.确认);
                    break;
                case ThemeColorType.sepline:
                    Configs.themes.sepline = color;
                    XGUI_Utilitys.Console("XHud 仪表盘消息", $"已将主题分割线颜色设为：{color}！", XGUIMsgState.确认);
                    break;
            }
            config_save();
        }
        /// <summary>
        /// 按预设名称设置主色调
        /// </summary>
        /// <param name="target">主色调预设名称</param>
        /// <returns>匹配到的预设颜色；未匹配到时返回默认颜色</returns>
        public static Color themes_set_primary_color(string target)
        {
            Color color = default;
            foreach (var item in Configs.themes.presets_primary)
            {
                if (target == item.name)
                {
                    color = item.color;
                    themes_set_color(ThemeColorType.primary, item.color);

                    XGUI_Utilitys.Console("XHud 仪表盘消息", $"已将主题主色调颜色设为：{target}！", XGUIMsgState.确认);
                }
            }
            return color;
        }
        /// <summary>
        /// 按预设名称设置编组色
        /// </summary>
        /// <param name="target">编组色预设名称</param>
        /// <returns>匹配到的预设颜色；未匹配到时返回默认颜色</returns>
        public static Color themes_set_group_color(string target)
        {
            Color color = default;
            foreach (var item in Configs.themes.presets_group)
            {
                if (target == item.name)
                {
                    color = item.color;
                    themes_set_color(ThemeColorType.group, item.color);

                    XGUI_Utilitys.Console("XHud 仪表盘消息", $"已将主题编组颜色设为：{target}！", XGUIMsgState.确认);
                }
            }
            return color;
        }
        /// <summary>
        /// 按预设名称设置分割线颜色
        /// </summary>
        /// <param name="target">分割线色预设名称</param>
        /// <returns>匹配到的预设颜色；未匹配到时返回默认颜色</returns>
        public static Color themes_set_sepline_color(string target)
        {
            Color color = default;
            foreach (var item in Configs.themes.presets_seperateline)
            {
                if (target == item.name)
                {
                    color = item.color;
                    themes_set_color(ThemeColorType.sepline, item.color);

                    XGUI_Utilitys.Console("XHud 仪表盘消息", $"已将主题分割线颜色设为：{target}！", XGUIMsgState.确认);
                }
            }
            return color;
        }
        /// <summary>获取主色调全部预设的名称数组</summary>
        /// <returns>按配置顺序排列的预设名称数组</returns>
        public static string[] themes_get_presetnames_primary()
        {
            List<string> names = new List<string>();
            foreach (var item in Configs.themes.presets_primary)
            {
                names.Add(item.name);
            }

            return names.ToArray();
        }
        /// <summary>获取编组色全部预设的名称数组</summary>
        /// <returns>按配置顺序排列的预设名称数组</returns>
        public static string[] themes_get_presetnames_group()
        {
            List<string> names = new List<string>();
            foreach (var item in Configs.themes.presets_group)
            {
                names.Add(item.name);
            }

            return names.ToArray();
        }
        /// <summary>获取分割线色全部预设的名称数组</summary>
        /// <returns>按配置顺序排列的预设名称数组</returns>
        public static string[] themes_get_presetnames_sepline()
        {
            List<string> names = new List<string>();
            foreach (var item in Configs.themes.presets_seperateline)
            {
                names.Add(item.name);
            }

            return names.ToArray();
        }
        #endregion

        #region 通用类配置 - 路径
        /// <summary>
        /// XHud 根目录：<c>Assets/SevenStrikeModules/XHud/</c>
        /// </summary>
        public static string path_XHUD_ROOT = "Assets/SevenStrikeModules/XHud/";
        /// <summary>
        /// GUI 根目录：<c>Assets/SevenStrikeModules/XHud/GUI/</c>
        /// </summary>
        public static string path_XHUD_GUIROOT = "Assets/SevenStrikeModules/XHud/GUI/";
        /// <summary>
        /// 配置目录：<c>Assets/SevenStrikeModules/XHud/Config/</c>
        /// </summary>
        public static string path_XHUD_CONFIG = "Assets/SevenStrikeModules/XHud/Config/";
        /// <summary>
        /// GUI 样式目录：<c>Assets/SevenStrikeModules/XHud/GUI/HudGuiStyle/</c>
        /// </summary>
        public static string path_XHUD_GUISTYLE = "Assets/SevenStrikeModules/XHud/GUI/HudGuiStyle/";
        /// <summary>
        /// 材质目录：<c>Assets/SevenStrikeModules/XHud/Materials/</c>
        /// </summary>
        public static string path_XHUD_MATERIAL = "Assets/SevenStrikeModules/XHud/Materials/";
        /// <summary>
        /// 预制体目录：<c>Assets/SevenStrikeModules/XHud/Prefabs/</c>
        /// </summary>
        public static string path_XHUD_PREFABS = "Assets/SevenStrikeModules/XHud/Prefabs/";
        /// <summary>
        /// 着色器目录：<c>Assets/SevenStrikeModules/XHud/Shaders/</c>
        /// </summary>
        public static string path_XHUD_SHADERS = "Assets/SevenStrikeModules/XHud/Shaders/";
        /// <summary>
        /// 音效目录：<c>Assets/SevenStrikeModules/XHud/Sound/</c>
        /// </summary>
        public static string path_XHUD_SOUND = "Assets/SevenStrikeModules/XHud/Sound/";
        /// <summary>
        /// 精灵目录：<c>Assets/SevenStrikeModules/XHud/Sprites/</c>
        /// </summary>
        public static string path_XHUD_SPRITES = "Assets/SevenStrikeModules/XHud/Sprites/";
        /// <summary>
        /// 贴图目录：<c>Assets/SevenStrikeModules/XHud/Textures/</c>
        /// </summary>
        public static string path_XHUD_TEXTURES = "Assets/SevenStrikeModules/XHud/Textures/";
        /// <summary>
        /// 字体目录：<c>Assets/SevenStrikeModules/XHud/Fonts/</c>
        /// </summary>
        public static string path_XHUD_FONTS = "Assets/SevenStrikeModules/XHud/Fonts/";
        /// <summary>
        /// 脚本目录：<c>Assets/SevenStrikeModules/XHud/Scripts/</c>
        /// </summary>
        public static string path_XHUD_SCRIPTS = "Assets/SevenStrikeModules/XHud/Scripts/";
        /// <summary>
        /// 第三方插件目录：<c>Assets/SevenStrikeModules/XHud/ThirdPlugin/</c>
        /// </summary>
        public static string path_XHUD_THIRDPLUGIN = "Assets/SevenStrikeModules/XHud/ThirdPlugin/";

        /// <summary>获取 XHud 根目录路径</summary>
        /// <returns><c>Assets/SevenStrikeModules/XHud/</c></returns>
        public static string Get_Path_XHUD_ROOT()
        {
            return path_XHUD_ROOT;
        }
        /// <summary>获取 XHud GUI 根目录路径</summary>
        /// <returns><c>Assets/SevenStrikeModules/XHud/GUI/</c></returns>
        public static string Get_Path_XHUD_GUIROOT_Path()
        {
            return path_XHUD_GUIROOT;
        }
        /// <summary>获取 XHud GUI 样式目录路径</summary>
        /// <returns><c>Assets/SevenStrikeModules/XHud/GUI/HudGuiStyle/</c></returns>
        public static string Get_Path_XHUD_GUISTYLE_Path()
        {
            return path_XHUD_GUISTYLE;
        }
        /// <summary>获取 XHud 配置目录路径</summary>
        /// <returns><c>Assets/SevenStrikeModules/XHud/Config/</c></returns>
        public static string Get_Path_XHUD_CONFIG_Path()
        {
            return path_XHUD_CONFIG;
        }
        /// <summary>获取 XHud 材质目录路径</summary>
        /// <returns><c>Assets/SevenStrikeModules/XHud/Materials/</c></returns>
        public static string Get_Path_XHUD_MATERIALS_Path()
        {
            return path_XHUD_MATERIAL;
        }
        /// <summary>获取 XHud 预制体目录路径</summary>
        /// <returns><c>Assets/SevenStrikeModules/XHud/Prefabs/</c></returns>
        public static string Get_Path_XHUD_PREFABS_Path()
        {
            return path_XHUD_PREFABS;
        }
        /// <summary>获取 XHud 着色器目录路径</summary>
        /// <returns><c>Assets/SevenStrikeModules/XHud/Shaders/</c></returns>
        public static string Get_Path_XHUD_SHADERS_Path()
        {
            return path_XHUD_SHADERS;
        }
        /// <summary>获取 XHud 音效目录路径</summary>
        /// <returns><c>Assets/SevenStrikeModules/XHud/Sound/</c></returns>
        public static string Get_Path_XHUD_SOUND_Path()
        {
            return path_XHUD_SOUND;
        }
        /// <summary>获取 XHud 精灵目录路径</summary>
        /// <returns><c>Assets/SevenStrikeModules/XHud/Sprites/</c></returns>
        public static string Get_Path_XHUD_SPRITES_Path()
        {
            return path_XHUD_SPRITES;
        }
        /// <summary>获取 XHud 贴图目录路径</summary>
        /// <returns><c>Assets/SevenStrikeModules/XHud/Textures/</c></returns>
        public static string Get_Path_XHUD_TEXTURES_Path()
        {
            return path_XHUD_TEXTURES;
        }
        /// <summary>获取 XHud 字体目录路径</summary>
        /// <returns><c>Assets/SevenStrikeModules/XHud/Fonts/</c></returns>
        public static string Get_Path_XHUD_FONTS_Path()
        {
            return path_XHUD_FONTS;
        }
        /// <summary>获取 XHud 脚本目录路径</summary>
        /// <returns><c>Assets/SevenStrikeModules/XHud/Scripts/</c></returns>
        public static string Get_Path_XHUD_SCRIPTS_Path()
        {
            return path_XHUD_SCRIPTS;
        }
        /// <summary>获取 XHud 第三方插件目录路径</summary>
        /// <returns><c>Assets/SevenStrikeModules/XHud/ThirdPlugin/</c></returns>
        public static string Get_Path_XHUD_THIRDPLUGIN_Path()
        {
            return path_XHUD_THIRDPLUGIN;
        }
        #endregion

        #region 元素类配置 - 动画预览选项

        /// <summary>获取“自动杀死预览动画”开关状态；配置缺失时返回 <c>false</c></summary>
        /// <returns>是否已开启自动杀死预览动画</returns>
        public static bool element_get_preview_autokill()
        {
            // 额外容错：避免加载配置失败后仍为null
            return Configs.elements?.previewtweens_autokill ?? false;
        }

        /// <summary>获取“预览杀死前重置动画”开关状态；配置缺失时返回 <c>false</c></summary>
        /// <returns>是否已开启杀死前重置动画</returns>
        public static bool element_get_preview_rewind_before_kill()
        {
            return Configs.elements?.previewtweens_rewindwithkill ?? false;
        }

        /// <summary>获取“预览杀死后清空预览列表”开关状态；配置缺失时返回 <c>false</c></summary>
        /// <returns>是否已开启杀死后清空预览列表</returns>
        public static bool element_get_preview_clear_after_kill()
        {
            return Configs.elements?.previewtweens_clearwithkill ?? false;
        }
#if UNITY_EDITOR
        /// <summary>
        /// 设置“自动杀死预览动画”开关并保存配置（仅编辑器）
        /// </summary>
        /// <param name="state">是否开启自动杀死预览动画</param>
        public static void element_set_preview_autokill(bool state)
        {
            Configs.elements.previewtweens_autokill = state;

            XGUI_Utilitys.Console("XHud 仪表盘消息", $"{(state ? "已开启" : "已关闭")}自动杀死预览动画！", XGUIMsgState.确认);

            config_save();
        }

        /// <summary>
        /// 设置“预览杀死前重置动画”开关并保存配置（仅编辑器）
        /// </summary>
        /// <param name="state">是否开启杀死前重置动画</param>
        public static void element_set_preview_rewind_before_kill(bool state)
        {
            Configs.elements.previewtweens_rewindwithkill = state;

            XGUI_Utilitys.Console("XHud 仪表盘消息", $"{(state ? "已开启" : "已关闭")}预览杀死前重置动画！", XGUIMsgState.确认);

            config_save();
        }

        /// <summary>
        /// 设置“预览杀死后清空预览列表”开关并保存配置（仅编辑器）
        /// </summary>
        /// <param name="state">是否开启杀死后清空预览列表</param>
        public static void element_set_preview_clear_after_kill(bool state)
        {
            Configs.elements.previewtweens_clearwithkill = state;

            XGUI_Utilitys.Console("XHud 仪表盘消息", $"{(state ? "已开启" : "已关闭")}预览杀死后清空预览列表！", XGUIMsgState.确认);

            config_save();
        }
#endif
        #endregion

        #region 图元类配置 - 场景中图元ID显示配置

        /// <summary>获取“场景中实时显示图元 ID”开关状态</summary>
        /// <returns>是否已开启图元 ID 显示</returns>
        public static bool primitive_get_controller_displayid()
        {
            return Configs.primitives.controller_id_displayer;
        }
#if UNITY_EDITOR
        /// <summary>
        /// 设置“场景中实时显示图元 ID”开关并保存配置（仅编辑器）
        /// </summary>
        /// <param name="state">是否开启图元 ID 显示</param>
        public static void primitive_set_controller_displayid(bool state)
        {
            Configs.primitives.controller_id_displayer = state;

            XGUI_Utilitys.Console("XHud 仪表盘消息", $"{(state ? "已开启" : "已关闭")}场景中图元ID实时显示！", XGUIMsgState.确认);

            config_save();
        }
#endif
        #endregion

        #region 工具配置 - 显示屏幕准心瞄具

        /// <summary>获取“显示屏幕准心瞄具”开关状态</summary>
        /// <returns>是否已开启屏幕准心瞄具</returns>
        public static bool utilitys_get_util_screeen_aimer()
        {
            return Configs.utilitys.util_screeen_aimer;
        }
#if UNITY_EDITOR
        /// <summary>
        /// 设置“显示屏幕准心瞄具”开关并保存配置（仅编辑器）
        /// </summary>
        /// <param name="state">是否开启屏幕准心瞄具</param>
        public static void utilitys_set_util_screeen_aimer(bool state)
        {
            Configs.utilitys.util_screeen_aimer = state;

            XGUI_Utilitys.Console("XHud 仪表盘消息", $"{(state ? "已开启" : "已关闭")}显示屏幕准心瞄具！", XGUIMsgState.确认);

            config_save();
        }
#endif
        #endregion

        #region 工具配置 - 元素通过拖拽放置到屏幕锚点

        /// <summary>获取“元素通过拖拽放置到屏幕锚点”开关状态</summary>
        /// <returns>是否已开启可视化放置工具</returns>
        public static bool utilitys_get_util_screeen_element_visual_placer()
        {
            return Configs.utilitys.util_screeen_element_visual_placer;
        }
#if UNITY_EDITOR
        /// <summary>
        /// 设置“元素通过拖拽放置到屏幕锚点”开关并保存配置（仅编辑器）
        /// </summary>
        /// <param name="state">是否开启可视化放置工具</param>
        public static void utilitys_set_util_screeen_element_visual_placer(bool state)
        {
            Configs.utilitys.util_screeen_element_visual_placer = state;

            XGUI_Utilitys.Console("XHud 仪表盘消息", $"{(state ? "已开启" : "已关闭")}元素通过拖拽放置到屏幕锚点！", XGUIMsgState.确认);

            config_save();
        }
#endif
        #endregion

        #region 配置文件操作
#if UNITY_EDITOR
        /// <summary>
        /// 从 <c>XHudConfigs.json</c> 载入配置；文件不存在时自动创建默认配置并保存（仅编辑器）
        /// </summary>
        /// <remarks>
        /// 通过 <see cref="DidReloadScripts"/> 在脚本重新编译后自动调用
        /// 载入成功后触发 <see cref="on_config_load"/>
        /// </remarks>
        [DidReloadScripts]
        public static void config_load()
        {
            // 读取配置json文件
            TextAsset json = AssetDatabase.LoadAssetAtPath<TextAsset>(Get_Path_XHUD_CONFIG_Path() + $"XHudConfigs.json");
            if (json == null)
            {
                Configs = config_create();
                config_save();
                json = AssetDatabase.LoadAssetAtPath<TextAsset>(Get_Path_XHUD_CONFIG_Path() + "XHudConfigs.json");
            }

            try
            {
                if (json != null)
                    Configs = JsonUtility.FromJson<XHudConfigs>(json.text);
                on_config_load?.Invoke(Configs);

                XGUI_Utilitys.Console("XHud 仪表盘消息", "已载入 XHud 配置！", XGUIMsgState.确认);
            }
            catch (Exception e) { Debug.LogError(e); }
        }

        /// <summary>
        /// 创建一份包含全部默认值的 XHud 根配置（含主题色与预设列表）
        /// </summary>
        /// <returns>新建的默认配置实例，同时会赋值给 <see cref="Configs"/> 并触发 <see cref="on_config_create"/></returns>
        public static XHudConfigs config_create()
        {
            XHudConfigs configs = new XHudConfigs();

            configs.version = "5.0";
            configs.licence = "unknow";
            configs.sub = "ui framework";
            configs.support = "此插件由 (南京塞维斯传媒) SevenStrikeMedia 独家开发";
            configs.intro = "HUD是一款专为Unity开发者设计的高效且易于使用的UI插件，旨在提供一套完整的用户界面解决方案，帮助开发者快速构建、定制和优化UI元素从初始化到调试，从视觉效果到交互功能，XHUD涵盖了一系列强大的工具和功能，满足不同开发需求，能够显著提升你的UI开发效率无论你是开发游戏、应用还是其他交互式项目，它都能为你提供强大的支持";
            configs.website = "http://sevenstrike.com";
            configs.document = "http://sevenstrike.com/col.jsp?id=103";

            Config_Utilitys con_util = new Config_Utilitys();
            con_util.util_screeen_aimer = false;
            con_util.util_screeen_element_visual_placer = false;

            Config_Elements con_ele = new Config_Elements();
            con_ele.previewtweens_autokill = true;
            con_ele.previewtweens_rewindwithkill = true;
            con_ele.previewtweens_clearwithkill = true;

            Config_Primitives con_prm = new Config_Primitives();
            con_prm.controller_id_displayer = false;

            Config_Themes con_thm = new Config_Themes();
            con_thm.primary = XGUI_Utilitys.HexString_To_Color("3BFE9B");
            con_thm.group = XGUI_Utilitys.HexString_To_Color("1E1E1E");
            con_thm.sepline = XGUI_Utilitys.HexString_To_Color("535353");

            #region 主要色预设
            List<Config_Color_Preset> con_presets_primary = new List<Config_Color_Preset>();

            List<colorpreset> presets_primary = new List<colorpreset>() {
                new colorpreset("默认", XGUI_Utilitys.HexString_To_Color("3BFE9B")),
                new colorpreset("白色", new Color(0.8196079f, 0.8196079f, 0.8196079f)),
                new colorpreset("黑色",new Color(0, 0, 0)),
                new colorpreset("乳白",new Color(0.8235295f, 0.7882354f, 0.7411765f)),
                new colorpreset("浅灰",new Color(0.5254902f, 0.5254902f, 0.5254902f)),
                new colorpreset("沙漠灰",new Color(0.9176471f, 0.8000001f, 0.5843138f)),
                new colorpreset("科幻青", new Color(0.2313726f, 0.9921569f, 0.6039216f)),
                new colorpreset("液晶绿", new Color(0.7254902f, 0.7882354f, 0.3568628f)),
                new colorpreset("橄榄绿",new Color(0.7176471f, 0.8235295f, 0.2039216f)),
                new colorpreset("湖蓝",new Color(0.2470588f, 0.6352941f, 0.9960785f)),
                new colorpreset("天蓝",new Color(0.4f, 0.6745098f, 0.854902f)),
                new colorpreset("胭脂粉", new Color(0.8588236f, 0.4901961f, 0.7294118f)),
                new colorpreset("灵动粉",new Color(0.882353f, 0.4705883f, 0.5215687f)),
                new colorpreset("秋叶黄", new Color(0.8352942f, 0.6117647f, 0.3411765f)),
                new colorpreset("警示黄",new Color(0.9960785f, 0.7686275f, 0)),
                new colorpreset("高亮橘",new Color(0.9960785f, 0.5176471f, 0.1686275f)),
                new colorpreset("烈焰红",new Color(0.9960785f, 0.3529412f, 0.2156863f))};

            foreach (var item in presets_primary)
            {
                Config_Color_Preset pr = new Config_Color_Preset();
                pr.name = item.name;
                pr.color = item.color;

                con_presets_primary.Add(pr.Clone());
            }
            #endregion

            #region 编组色预设
            List<Config_Color_Preset> con_presets_group = new List<Config_Color_Preset>();

            List<colorpreset> presets_group = new List<colorpreset>() {
                new colorpreset("默认", XGUI_Utilitys.HexString_To_Color("1E1E1E")),
                new colorpreset("白色", new Color(0.5568628f, 0.5568628f, 0.5568628f)),
                new colorpreset("浅灰", new Color(0.4f, 0.4f, 0.4f)),
                new colorpreset("深灰", new Color(0.1176471f, 0.1176471f, 0.1176471f)),
                new colorpreset("黑色", new Color(0, 0, 0)),
                new colorpreset("极简", new Color(0.2196079f, 0.2196079f, 0.2196079f)),
                new colorpreset("珊瑚红", new Color(0.5529412f, 0.3058824f, 0.3058824f)),
                new colorpreset("落叶黄", new Color(0.5529412f, 0.4470589f, 0.3058824f)),
                new colorpreset("烟灰蓝", new Color(0.2980392f, 0.3568628f, 0.427451f)),
                new colorpreset("青苔绿", new Color(0.3529412f, 0.4039216f, 0.2470588f)),
                new colorpreset("荧光绿", new Color(0.3058824f, 0.427451f, 0.2235294f)),
                new colorpreset("淡粉", new Color(0.427451f, 0.3058824f, 0.4078432f) )};

            foreach (var item in presets_group)
            {
                Config_Color_Preset pr = new Config_Color_Preset();
                pr.name = item.name;
                pr.color = item.color;

                con_presets_group.Add(pr.Clone());
            }
            #endregion

            #region 分割线色预设
            List<Config_Color_Preset> con_presets_sepline = new List<Config_Color_Preset>();

            List<colorpreset> presets_sepline = new List<colorpreset>() {
                new colorpreset("默认", XGUI_Utilitys.HexString_To_Color("535353")),
                new colorpreset("白色", new Color(0.5568628f, 0.5568628f, 0.5568628f)),
                new colorpreset("浅灰", new Color(0.4f, 0.4f, 0.4f)),
                new colorpreset("深灰", new Color(0.1176471f, 0.1176471f, 0.1176471f)),
                new colorpreset("黑色", new Color(0, 0, 0)),
                new colorpreset("极简", new Color(0.2196079f, 0.2196079f, 0.2196079f)),
                new colorpreset("珊瑚红", new Color(0.5529412f, 0.3058824f, 0.3058824f)),
                new colorpreset("落叶黄", new Color(0.5529412f, 0.4470589f, 0.3058824f)),
                new colorpreset("烟灰蓝", new Color(0.2980392f, 0.3568628f, 0.427451f)),
                new colorpreset("青苔绿", new Color(0.3529412f, 0.4039216f, 0.2470588f)),
                new colorpreset("荧光绿", new Color(0.3058824f, 0.427451f, 0.2235294f)),
                new colorpreset("淡粉", new Color(0.427451f, 0.3058824f, 0.4078432f) )};

            foreach (var item in presets_sepline)
            {
                Config_Color_Preset pr = new Config_Color_Preset();
                pr.name = item.name;
                pr.color = item.color;

                con_presets_sepline.Add(pr.Clone());
            }
            #endregion

            con_thm.presets_primary = con_presets_primary;
            con_thm.presets_group = con_presets_group;
            con_thm.presets_seperateline = con_presets_sepline;

            configs.utilitys = con_util;
            configs.elements = con_ele;
            configs.primitives = con_prm;
            configs.themes = con_thm;

            on_config_create?.Invoke(configs);

            Configs = configs;

            XGUI_Utilitys.Console("XHud 仪表盘消息", "已创建 XHud 配置！", XGUIMsgState.确认);

            return configs;
        }

        /// <summary>
        /// 获取当前配置的一份深拷贝（调用方可安全修改，不影响全局配置）
        /// </summary>
        /// <returns><see cref="Configs"/> 的深拷贝实例</returns>
        public static XHudConfigs config_get()
        {
            return Configs.Clone();
        }

        /// <summary>
        /// 将当前配置序列化为 JSON 并写入 <c>XHudConfigs.json</c>，同时刷新资源并触发 <see cref="on_config_save"/>（仅编辑器）
        /// </summary>
        public static void config_save()
        {
            string json = JsonUtility.ToJson(Configs);

            // 使用StreamWriter写入文件
            using (StreamWriter writer = new StreamWriter(Get_Path_XHUD_CONFIG_Path() + $"XHudConfigs.json"))
            {
                writer.Write(json);
            }

            // 刷新文件资源
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.SetDirty(AssetDatabase.LoadAssetAtPath<TextAsset>(Get_Path_XHUD_CONFIG_Path() + $"XHudConfigs.json"));

            XGUI_Utilitys.Console("XHud 仪表盘消息", "已保存 XHud 配置！", XGUIMsgState.确认);

            on_config_save?.Invoke(Configs);
        }
#endif
        #endregion
    }
}