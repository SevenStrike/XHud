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
    using UnityEngine;

    /// <summary>
    /// 单向只读适配器抽象基类。
    /// <para/>
    /// 负责处理数据源（Category）的生命周期绑定、断线重连、初始值回显，并向子类提供统一的数据接入接口。
    /// 所有面向 UI 控件的单向绑定（如 Text、Image、Slider 的显示层）都应继承自此类。
    /// </summary>
    /// <typeparam name="T">绑定的数据类型</typeparam>
    public abstract class XBinder_Adapter_Base<T> : MonoBehaviour
    {
        #region 字段
        /// <summary>
        /// 运行时的真实数据源。
        /// <para/>
        /// 由基类在绑定成功时自动赋值，断线或禁用时清空。
        /// </summary>
        protected XBinder_Category category { get; private set; }
        /// <summary>
        /// 目标数据容器名（不带前缀）。
        /// </summary>
        [SerializeField] public string target_category = "";
        /// <summary>
        /// 目标字段键名。
        /// </summary>
        [SerializeField] public string target_key = "";
        /// <summary>
        /// 防循环标记（供双向绑定使用）。
        /// <para/>
        /// 当该值为 true 时，表示当前正处于自身回写状态，应拦截 on_value_changed 向外派发。
        /// </summary>
        protected bool _syncing = false;
        /// <summary>
        /// 是否已成功绑定数据源。
        /// </summary>
        protected bool _has_bound = false;
        private bool _initial_pushed = false;
        #endregion

        #region 生命周期
        /// <summary>
        /// 启用时监听总线事件，并尝试首次绑定。
        /// </summary>
        protected virtual void OnEnable()
        {
            // 监听总线事件
            XBinder_Bus.on_category_created += handle_category_created;
            XBinder_Bus.on_category_destroyed += handle_category_destroyed;

            bind();
        }
        /// <summary>
        /// 禁用时取消总线监听，并主动解绑。
        /// </summary>
        protected virtual void OnDisable()
        {
            // 取消监听，防止内存泄漏
            XBinder_Bus.on_category_created -= handle_category_created;
            XBinder_Bus.on_category_destroyed -= handle_category_destroyed;

            unbind();
        }
        /// <summary>
        /// 每帧兜底检测。如果已经绑定成功，但还没推送过初始值，就在这一帧补上。
        /// </summary>
        protected virtual void Update()
        {
            if (_has_bound && !_initial_pushed)
            {
                _initial_pushed = true;
                DoPushInitialValue();
            }
        }
        #endregion

        #region 数据绑定与重连核心
        /// <summary>
        /// 从总线获取数据源并绑定。
        /// <para/>
        /// 若绑定成功，会立即调用 <see cref="get_value()"/> 并派发初始值，完成 UI 的首次刷新。
        /// </summary>
        private void bind()
        {
            if (!string.IsNullOrEmpty(target_category))
            {
                category = XBinder_Bus.get_category_by_name(target_category);
            }

            if (category != null && !_has_bound)
            {
                _has_bound = true;
                // 绑定成功时重置
                _initial_pushed = false;
                subscribe();
            }
        }
        /// <summary>
        /// 内部主动解绑（供 OnDisable 和 断线时复用）。
        /// <para/>
        /// 清空 <c>category</c> 引用，等待总线事件重新触发绑定。
        /// </summary>
        private void unbind()
        {
            if (_has_bound && category != null)
            {
                unsubscribe();
                _has_bound = false;
            }
            category = null;
            // 解绑时重置
            _initial_pushed = false;
        }
        /// <summary>
        /// 处理总线数据源重生事件。
        /// <para/>
        /// 仅当重生的是当前适配器正在等待的目标时，才尝试重新绑定。
        /// </summary>
        /// <param name="created_name">被创建的数据源全名（带前缀）</param>
        protected virtual void handle_category_created(string created_name)
        {
            // 如果重生的数据源正是我一直在等的目标，且我当前没绑上
            if (!_has_bound && $"xbinder_{target_category}" == created_name)
            {
                bind();

                // 如果是运行中途重生重连，业务层肯定已经准备好了，直接推
                if (_has_bound)
                {
                    // 重连推之前打个标记
                    _initial_pushed = true;
                    DoPushInitialValue();
                }
            }
        }
        /// <summary>
        /// 统一处理总线数据源被卸载的事件。
        /// <para/>
        /// 若被卸载的正是当前绑定的数据源，则主动解绑，等待后续重生。
        /// </summary>
        /// <param name="destroyed_name">被销毁的数据源全名（带前缀）</param>
        protected virtual void handle_category_destroyed(string destroyed_name)
        {
            if ($"xbinder_{target_category}" == destroyed_name)
            {
                unbind();
            }
        }
        private void DoPushInitialValue()
        {
            bool pre_state = _syncing;
            _syncing = false;
            on_value_changed(get_value());
            _syncing = pre_state;
        }
        #endregion

        #region 子类必须实现的方法
        /// <summary>
        /// 向数据源注册监听。
        /// <para/>
        /// 仅在成功绑定数据源后由基类自动调用一次。
        /// </summary>
        protected abstract void subscribe();
        /// <summary>
        /// 从数据源注销监听。
        /// <para/>
        /// 在禁用、销毁、数据源断线时由基类自动调用。
        /// </summary>
        protected abstract void unsubscribe();
        /// <summary>
        /// 获取数据源中当前目标键的值。
        /// <para/>
        /// 由基类在初次绑定成功时调用，用于初始 UI 刷新。
        /// </summary>
        /// <returns>数据源中存储的当前值</returns>
        public abstract T get_value();
        /// <summary>
        /// 收到数据变化时的回调。
        /// <para/>
        /// 由基类在数据源推送新值或初次绑定时调用，子类在此处将值应用到具体的 UI 控件上。
        /// </summary>
        /// <param name="value">数据源推送的新值</param>
        protected abstract void on_value_changed(T value);
        #endregion
    }
}