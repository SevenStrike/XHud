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

    /// <summary>
    /// 双向同步适配器抽象基类。
    /// <para/>
    /// 在 <see cref="XBinder_Adapter_Base{T}"/>（单向只读）的基础上，增加了“回写”能力，
    /// 用于 Slider、Toggle、InputField 等需要把用户操作写回数据源的 UI 控件。
    /// <para/>
    /// 该类通过 <c>_syncing</c> 闸门拦截自身回环，确保“UI → 数据”与“数据 → UI”两条链路互不干扰。
    /// </summary>
    /// <typeparam name="T">绑定的数据类型</typeparam>
    public abstract class XBinder_Adapter_Sync<T> : XBinder_Adapter_Base<T>
    {
        #region 字段
        /// <summary>
        /// 数据变化时的代码层订阅入口。
        /// <para/>
        /// 与 <see cref="XBinder_Adapter_Base{T}.on_value_changed"/> 不同，此事件已经过滤了自身回写触发的循环回调。
        /// </summary>
        public event Action<T> act_on_value_changed;
        /// <summary>
        /// 当数据源重新连接时
        /// </summary>
        public Action<string> act_handle_category_created;
        /// <summary>
        /// 当数据源断开时
        /// </summary>
        public Action<string> act_handle_category_destroyed;
        #endregion

        #region 折叠
        public bool
            fold_param = false,
            fold_based = true;

        public void GroupFold(bool state)
        {
            fold_param = state;
            fold_based = state;
        }

        public void GroupFold(bool param = true, bool based = true)
        {
            fold_param = param;
            fold_based = based;
        }
        #endregion

        #region 回写入口
        /// <summary>
        /// 外部逻辑回写数据到数据源。
        /// <para/>
        /// 当 UI 控件被用户操作（如拖动 Slider）时，由子类调用此方法，将新值写入 Category。
        /// <para/>
        /// 内部会开启 <c>_syncing</c> 闸门，确保写入动作触发的 <see cref="on_value_changed"/> 不会再次派发给外部订阅者，从而避免循环触发。
        /// </summary>
        /// <param name="value">要回写的数据值</param>
        public void push(T value)
        {
            if (category == null)
            {
                XGUI_Utilitys.Console("XBinder 消息", $"[XBinder_Adapter_Sync] 回写被丢弃：未绑定数据源 (category={target_category}, key={target_key})", XGUIMsgState.警告);
                return;
            }

            _syncing = true;
            try
            {
                set_value(value);
            }
            finally
            {
                _syncing = false;
            }
        }
        /// <summary>
        /// 子类实现：把值写回 Category 通道。
        /// <para/>
        /// 由 <see cref="push(T)"/> 调用，受 <c>_syncing</c> 闸门保护。
        /// </summary>
        /// <param name="value">要写入通道的值</param>
        protected abstract void set_value(T value);
        #endregion

        #region 数据出口
        /// <summary>
        /// 接收数据源变化，过滤自身回环后向外分发。
        /// <para/>
        /// 若当前处于 <c>_syncing</c> 状态（即变化来自 <see cref="push(T)"/>），则直接丢弃，
        /// 防止“UI 改 → 数据变 → UI 再改”的死循环。
        /// </summary>
        /// <param name="value">数据源推送的新值</param>
        protected override void on_value_changed(T value)
        {
            if (_syncing) return;

            act_on_value_changed?.Invoke(value);
        }
        #endregion

        #region 断线/重连表现
        /// <summary>
        /// 当数据源重新连接时
        /// </summary>
        protected override void handle_category_created(string name)
        {
            base.handle_category_created(name);

            act_handle_category_created?.Invoke(name);
        }
        /// <summary>
        /// 当数据源断开时
        /// </summary>
        protected override void handle_category_destroyed(string name)
        {
            base.handle_category_destroyed(name);
            act_handle_category_destroyed?.Invoke(name);
        }
        #endregion
    }
}