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
    using System;

    /// <summary>
    /// object 类型的双向同步适配器。
    /// <para/>
    /// 该类是 <see cref="XBinder_Adapter_Sync{T}"/> 针对 object 类型的“叶子节点”实现。
    /// 负责将 object 类型的数据与 <see cref="XBinder_Category"/> 中的 object 通道进行双向对接。
    /// </summary>
    [Serializable]
    public class XBinder_Adapter_Sync_Object : XBinder_Adapter_Sync<object>
    {
        /// <summary>
        /// 向数据源注册 object 监听。
        /// <para/>
        /// 由基类在成功绑定数据源时自动调用。
        /// </summary>
        protected override void subscribe()
        {
            category.subscribe_object(target_key, on_value_changed);
        }
        /// <summary>
        /// 从数据源注销 object 监听。
        /// <para/>
        /// 由基类在禁用、销毁或数据源断线时自动调用。
        /// </summary>
        protected override void unsubscribe()
        {
            category.unsubscribe_object(target_key, on_value_changed);
        }
        /// <summary>
        /// 获取数据源中该目标键的当前 object 值。
        /// <para/>
        /// 由基类在初次绑定成功时调用，用于强制刷新初始 UI 状态。
        /// </summary>
        /// <returns>当前 object 值；若键不存在则返回 null</returns>
        public override object get_value()
        {
            return category.get_object(target_key);
        }
        /// <summary>
        /// 将外部传入的 object 值回写到数据源中。
        /// <para/>
        /// 由 <see cref="XBinder_Adapter_Sync{T}.push(T)"/> 触发，受 <c>_syncing</c> 闸门保护，避免双向循环死锁。
        /// </summary>
        /// <param name="v">要写回的 object 值</param>
        protected override void set_value(object v)
        {
            category.set_object(target_key, v);
        }
    }
}