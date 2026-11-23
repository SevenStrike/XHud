/*
 * ============================================================================
 * ⚠ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XBinder - Unity 高级数据绑定管理插件
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
namespace SevenStrikeModules.XBinder
{
    using SevenStrikeModules.XGUI.Runtime;
    using System;
    using System.Collections.Generic;

    public static class XBinder_Bus
    {
        #region 字段
        /// <summary>
        /// 登记在册的所有数据源列表
        /// </summary>
        public static readonly List<XBinder_Category> categories = new();
        /// <summary>
        /// 当有新的数据源被创建并注册到总线时触发。
        /// <para/>
        /// 参数为数据源的完整显示名（带 "xbinder_" 前缀）。
        /// </summary>
        public static event Action<string> on_category_created;
        /// <summary>
        /// 当有数据源从总线被销毁并注销时触发。
        /// <para/>
        /// 参数为数据源的完整显示名（带 "xbinder_" 前缀）。
        /// </summary>
        public static event Action<string> on_category_destroyed;
        #endregion

        #region 注册与注销
        /// <summary>
        /// 向数据总线注册一个数据源。
        /// </summary>
        /// <param name="category">要注册的数据源实例</param>
        public static void register(XBinder_Category category)
        {
            if (category != null && !categories.Contains(category))
            {
                categories.Add(category);
            }
        }
        /// <summary>
        /// 从数据总线注销一个数据源。
        /// </summary>
        /// <param name="category">要注销的数据源实例</param>
        public static void unregister(XBinder_Category category)
        {
            if (category != null && categories.Contains(category))
            {
                categories.Remove(category);
            }
        }
        #endregion

        #region 绑定容器操作
        /// <summary>
        /// 创建一个绑定容器，并立即注册到总线。
        /// <para/>
        /// 创建成功后会广播 <see cref="on_category_created"/> 事件。
        /// </summary>
        /// <param name="name">数据源标识名（不带前缀）</param>
        /// <returns>新建的数据源实例</returns>
        public static XBinder_Category create_category(string name)
        {
            XBinder_Category category = new XBinder_Category();
            category.display_name = "xbinder_" + name;

            // 注册到总线中
            XBinder_Bus.register(category);

            // 广播诞生事件（注意：传带前缀的名字，和 destroyed 保持一致）
            if (on_category_created != null)
                on_category_created(category.display_name);

            return category;
        }
        /// <summary>
        /// 销毁一个绑定容器并从总线注销。
        /// <para/>
        /// 注销后会广播 <see cref="on_category_destroyed"/> 事件，通知所有适配器断线。
        /// </summary>
        /// <param name="category">要销毁的数据源实例</param>
        public static void destroy_category(XBinder_Category category)
        {
            if (category == null) return;
            unregister(category);
            // 在 C# 中不用手动置 null，unregister 移除引用后，GC 自然会回收

            if (on_category_destroyed != null)
                on_category_destroyed(category.display_name);
        }
        #endregion

        #region 查询方法
        /// <summary>
        /// 打印当前总线上所有数据源的名称。
        /// </summary>
        public static void get_category_names()
        {
            for (int i = 0; i < categories.Count; i++)
            {
                XGUI_Utilitys.Console("XBinder 消息", $"数据容器: {categories[i].display_name}", XGUIMsgState.通知);
            }
        }
        /// <summary>
        /// 根据名称从总线获取对应的数据源。
        /// </summary>
        /// <param name="name">数据源的显示名称（不带前缀）</param>
        /// <returns>匹配的数据源实例，未找到则返回 null</returns>
        public static XBinder_Category get_category_by_name(string name)
        {
            foreach (var cat in categories)
                if (cat.display_name == "xbinder_" + name) return cat;
            return null;
        }
        /// <summary>
        /// 根据名称获取数据源在总线列表中的索引。
        /// </summary>
        /// <param name="name">数据源的显示名称（不带前缀）</param>
        /// <returns>数据源在列表中的索引，未找到则返回 0</returns>
        public static int get_category_index(string name)
        {
            int index = 0;
            for (int i = 0; i < categories.Count; i++)
            {
                if (categories[i].display_name == "xbinder_" + name)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
        #endregion
    }
}