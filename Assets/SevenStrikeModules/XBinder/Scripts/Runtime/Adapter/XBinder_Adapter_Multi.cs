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
    using UnityEngine;

    /// <summary>
    /// 多路绑定条目配置。
    /// <para/>
    /// 每个 entry 对应数据源中一个 key 的订阅，由 <see cref="XBinder_Adapter_Multi"/> 在构建时批量处理。
    /// </summary>
    [Serializable]
    public class XBinder_Adapter_BindEntry
    {
        /// <summary>
        /// 目标字段键名
        /// </summary>
        public string target_key = "";

        /// <summary>
        /// 目标数据容器名
        /// </summary>
        public string target_category = "";

        /// <summary>
        /// 绑定的数据类型
        /// </summary>
        public XBinder_ValueType value_type = XBinder_ValueType.Float;
    }

    /// <summary>
    /// 多路单向适配器。
    /// <para/>
    /// 通过 <c>entries</c> 配置批量订阅多个 key，并通过 <c>on_float</c>、<c>on_string</c> 等事件将数据分发到业务层。
    /// 支持断线重连与局部解绑，内部完全事件驱动，无 Update 轮询。
    /// </summary>
    public class XBinder_Adapter_Multi : MonoBehaviour
    {
        #region 字段
        /// <summary>
        /// 需要绑定的数据列表（每个 entry 对应一个 key 的订阅）
        /// </summary>
        [Tooltip("每个 entry = 一个数据 key 的订阅；具体写到哪里由外部事件处理")]
        public List<XBinder_Adapter_BindEntry> entries = new();
        /// <summary>
        /// float 数据事件，参数为（key, value）
        /// </summary>
        public event Action<string, float> on_float;
        /// <summary>
        /// int 数据事件，参数为（key, value）
        /// </summary>
        public event Action<string, int> on_int;
        /// <summary>
        /// bool 数据事件，参数为（key, value）
        /// </summary>
        public event Action<string, bool> on_bool;
        /// <summary>
        /// string 数据事件，参数为（key, value）
        /// </summary>
        public event Action<string, string> on_string;
        /// <summary>
        /// Color 数据事件，参数为（key, value）
        /// </summary>
        public event Action<string, Color> on_color;
        /// <summary>
        /// Vector2 数据事件，参数为（key, value）
        /// </summary>
        public event Action<string, Vector2> on_vector2;
        /// <summary>
        /// Vector3 数据事件，参数为（key, value）
        /// </summary>
        public event Action<string, Vector3> on_vector3;
        /// <summary>
        /// Vector4 数据事件，参数为（key, value）
        /// </summary>
        public event Action<string, Vector4> on_vector4;
        /// <summary>
        /// Quaternion 数据事件，参数为（key, value）
        /// </summary>
        public event Action<string, Quaternion> on_quaternion;
        /// <summary>
        /// Rect 数据事件，参数为（key, value）
        /// </summary>
        public event Action<string, Rect> on_rect;
        /// <summary>
        /// Object 数据事件，参数为（key, value）
        /// </summary>
        public event Action<string, object> on_object;
        /// <summary>
        /// 全局目标容器名。
        /// <para/>
        /// 当 entry 未单独指定 global_category 时，使用此值作为默认容器。
        /// </summary>
        public string global_category;
        /// <summary>
        /// 全局目标容器名。
        /// <para/>
        /// 标识名称
        /// </summary>
        public string identifier;
        #endregion

        #region 折叠
        public bool
            fold_param = true,
            fold_list = true,
            fold_based = true;

        public void GroupFold(bool state)
        {
            fold_param = state;
            fold_list = state;
            fold_based = state;
        }

        public void GroupFold(bool param = true, bool list = true, bool based = true)
        {
            fold_param = param;
            fold_list = list;
            fold_based = based;
        }
        #endregion

        /// <summary>
        /// 运行时绑定条目。
        /// <para/>
        /// 由配置 entry 构建，包含数据源引用、订阅状态以及各类型的回调缓存。
        /// </summary>
        [Serializable]
        public class bind_item
        {
            /// <summary>
            /// 配置数据
            /// </summary>
            public XBinder_Adapter_BindEntry cfg;

            /// <summary>
            /// 数据源实例
            /// </summary>
            public XBinder_Category category;

            /// <summary>
            /// 是否已绑定
            /// </summary>
            public bool bound;

            /// <summary>
            /// float 回调缓存
            /// </summary>
            public Action<float> cb_float;

            /// <summary>
            /// int 回调缓存
            /// </summary>
            public Action<int> cb_int;

            /// <summary>
            /// bool 回调缓存
            /// </summary>
            public Action<bool> cb_bool;

            /// <summary>
            /// string 回调缓存
            /// </summary>
            public Action<string> cb_string;

            /// <summary>
            /// Color 回调缓存
            /// </summary>
            public Action<Color> cb_color;

            /// <summary>
            /// Vector2 回调缓存
            /// </summary>
            public Action<Vector2> cb_vector2;

            /// <summary>
            /// Vector3 回调缓存
            /// </summary>
            public Action<Vector3> cb_vector3;

            /// <summary>
            /// Vector4 回调缓存
            /// </summary>
            public Action<Vector4> cb_vector4;

            /// <summary>
            /// Quaternion 回调缓存
            /// </summary>
            public Action<Quaternion> cb_quaternion;

            /// <summary>
            /// Rect 回调缓存
            /// </summary>
            public Action<Rect> cb_rect;

            /// <summary>
            /// Object 回调缓存
            /// </summary>
            public Action<object> cb_object;
        }

        #region 运行时结构
        /// <summary>
        /// 运行时条目列表
        /// </summary>
        protected readonly List<bind_item> bind_items = new();
        /// <summary>
        /// 运行时是否已构建
        /// </summary>
        protected bool items_binded;
        #endregion

        #region 生命周期
        /// <summary>
        /// 启用时构建运行时条目，并尝试与总线中的数据进行绑定。
        /// </summary>
        protected virtual void OnEnable()
        {
            // 监听总线事件
            XBinder_Bus.on_category_created += handle_category_created;
            XBinder_Bus.on_category_destroyed += handle_category_destroyed;

            build();

            for (int i = 0; i < bind_items.Count; i++)
                bind(bind_items[i]);
        }
        /// <summary>
        /// 禁用时解除所有绑定，并取消总线事件监听。
        /// </summary>
        protected virtual void OnDisable()
        {
            // 取消监听，防止内存泄漏
            XBinder_Bus.on_category_created -= handle_category_created;
            XBinder_Bus.on_category_destroyed -= handle_category_destroyed;

            for (int i = 0; i < bind_items.Count; i++)
                unbind(bind_items[i]);
        }
        #endregion

        #region 构建
        /// <summary>
        /// 根据配置构建运行时条目。
        /// <para/>
        /// 该过程会进行配置校验、去重，并为每个条目分配对应类型的回调缓存。
        /// 仅执行一次，由 <c>items_binded</c> 防止重复构建。
        /// </summary>
        protected virtual void build()
        {
            if (items_binded) return;
            items_binded = true;

            HashSet<(string, string, XBinder_ValueType)> seen = new();
            for (int i = 0; i < entries.Count; i++)
            {
                var cfg = entries[i];
                if (cfg == null) continue;

                string tar_category = !string.IsNullOrEmpty(cfg.target_category) ? cfg.target_category : global_category;

                if (string.IsNullOrEmpty(cfg.target_category))
                {
                    XGUI_Utilitys.Console("XBinder 消息", $"[XBinder_Adapter_Multi] 绑定项 {cfg.target_key} 的 category_name 为空，使用全局目标容器名称 ：{global_category}", XGUIMsgState.确认);
                }

                if (string.IsNullOrEmpty(tar_category))
                {
                    XGUI_Utilitys.Console("XBinder 消息", $"[XBinder_Adapter_Multi] 跳过 entry：绑定项 {cfg.target_key} 的 category_name & 全局目标容器名称都为空", XGUIMsgState.警告);
                    continue;
                }

                if (string.IsNullOrEmpty(cfg.target_key))
                {
                    XGUI_Utilitys.Console("XBinder 消息", $"[XBinder_Adapter_Multi] 跳过 entry：绑定项的 key 为空", XGUIMsgState.警告);
                    continue;
                }

                if (!seen.Add((tar_category, cfg.target_key, cfg.value_type)))
                {
                    XGUI_Utilitys.Console("XBinder 消息", $"[XBinder_Adapter_Multi] 重复订阅: {cfg.target_category}.{cfg.target_key}({cfg.value_type})，已保留首条", XGUIMsgState.警告);
                    continue;
                }

                var item = new bind_item { cfg = cfg };
                switch (cfg.value_type)
                {
                    case XBinder_ValueType.Float: item.cb_float = v => on_float?.Invoke(cfg.target_key, v); break;
                    case XBinder_ValueType.Int: item.cb_int = v => on_int?.Invoke(cfg.target_key, v); break;
                    case XBinder_ValueType.Bool: item.cb_bool = v => on_bool?.Invoke(cfg.target_key, v); break;
                    case XBinder_ValueType.String: item.cb_string = v => on_string?.Invoke(cfg.target_key, v); break;
                    case XBinder_ValueType.Color: item.cb_color = v => on_color?.Invoke(cfg.target_key, v); break;
                    case XBinder_ValueType.Vector2: item.cb_vector2 = v => on_vector2?.Invoke(cfg.target_key, v); break;
                    case XBinder_ValueType.Vector3: item.cb_vector3 = v => on_vector3?.Invoke(cfg.target_key, v); break;
                    case XBinder_ValueType.Vector4: item.cb_vector4 = v => on_vector4?.Invoke(cfg.target_key, v); break;
                    case XBinder_ValueType.Quaternion: item.cb_quaternion = v => on_quaternion?.Invoke(cfg.target_key, v); break;
                    case XBinder_ValueType.Rect: item.cb_rect = v => on_rect?.Invoke(cfg.target_key, v); break;
                    case XBinder_ValueType.Object: item.cb_object = v => on_object?.Invoke(cfg.target_key, v); break;
                }
                bind_items.Add(item);
            }
        }
        #endregion

        #region 绑定
        /// <summary>
        /// 尝试绑定指定条目。
        /// <para/>
        /// 若目标数据源存在且尚未绑定，则注册监听并立即推送初始值。
        /// </summary>
        /// <param name="item">运行时绑定条目</param>
        protected virtual void bind(bind_item item)
        {
            if (item.bound) return;
            item.category = XBinder_Bus.get_category_by_name(!string.IsNullOrEmpty(item.cfg.target_category) ? item.cfg.target_category : global_category);
            if (item.category == null) return;

            subscribe(item);

            item.bound = true;

            // 直接回显初始值
            push_initial_value(item);
        }
        /// <summary>
        /// 注销指定条目的监听。
        /// </summary>
        /// <param name="item">运行时绑定条目</param>
        protected virtual void unbind(bind_item item)
        {
            if (!item.bound || item.category == null) return;

            unsubscribe(item);

            item.bound = false;
            item.category = null;
        }
        /// <summary>
        /// 推送指定条目的初始值。
        /// <para/>
        /// 在绑定成功时调用，确保 UI 立即显示当前数据源中的最新值。
        /// </summary>
        /// <param name="item">运行时绑定条目</param>
        protected virtual void push_initial_value(bind_item item)
        {
            switch (item.cfg.value_type)
            {
                case XBinder_ValueType.Float:
                    on_float?.Invoke(item.cfg.target_key, item.category.get_float(item.cfg.target_key));
                    break;
                case XBinder_ValueType.Int:
                    on_int?.Invoke(item.cfg.target_key, item.category.get_int(item.cfg.target_key));
                    break;
                case XBinder_ValueType.Bool:
                    on_bool?.Invoke(item.cfg.target_key, item.category.get_bool(item.cfg.target_key));
                    break;
                case XBinder_ValueType.String:
                    on_string?.Invoke(item.cfg.target_key, item.category.get_string(item.cfg.target_key));
                    break;
                case XBinder_ValueType.Color:
                    on_color?.Invoke(item.cfg.target_key, item.category.get_color(item.cfg.target_key));
                    break;
                case XBinder_ValueType.Vector2:
                    on_vector2?.Invoke(item.cfg.target_key, item.category.get_vector2(item.cfg.target_key));
                    break;
                case XBinder_ValueType.Vector3:
                    on_vector3?.Invoke(item.cfg.target_key, item.category.get_vector3(item.cfg.target_key));
                    break;
                case XBinder_ValueType.Vector4:
                    on_vector4?.Invoke(item.cfg.target_key, item.category.get_vector4(item.cfg.target_key));
                    break;
                case XBinder_ValueType.Quaternion:
                    on_quaternion?.Invoke(item.cfg.target_key, item.category.get_quaternion(item.cfg.target_key));
                    break;
                case XBinder_ValueType.Rect:
                    on_rect?.Invoke(item.cfg.target_key, item.category.get_rect(item.cfg.target_key));
                    break;
                case XBinder_ValueType.Object:
                    on_object?.Invoke(item.cfg.target_key, item.category.get_object(item.cfg.target_key));
                    break;
            }
        }
        /// <summary>
        /// 向数据源注册监听。
        /// </summary>
        /// <param name="item">运行时绑定条目</param>
        protected virtual void subscribe(bind_item item)
        {
            switch (item.cfg.value_type)
            {
                case XBinder_ValueType.Float:
                    item.category.subscribe_float(item.cfg.target_key, item.cb_float);
                    break;
                case XBinder_ValueType.Int:
                    item.category.subscribe_int(item.cfg.target_key, item.cb_int);
                    break;
                case XBinder_ValueType.Bool:
                    item.category.subscribe_bool(item.cfg.target_key, item.cb_bool);
                    break;
                case XBinder_ValueType.String:
                    item.category.subscribe_string(item.cfg.target_key, item.cb_string);
                    break;
                case XBinder_ValueType.Color:
                    item.category.subscribe_color(item.cfg.target_key, item.cb_color);
                    break;
                case XBinder_ValueType.Vector2:
                    item.category.subscribe_vector2(item.cfg.target_key, item.cb_vector2);
                    break;
                case XBinder_ValueType.Vector3:
                    item.category.subscribe_vector3(item.cfg.target_key, item.cb_vector3);
                    break;
                case XBinder_ValueType.Vector4:
                    item.category.subscribe_vector4(item.cfg.target_key, item.cb_vector4);
                    break;
                case XBinder_ValueType.Quaternion:
                    item.category.subscribe_quaternion(item.cfg.target_key, item.cb_quaternion);
                    break;
                case XBinder_ValueType.Rect:
                    item.category.subscribe_rect(item.cfg.target_key, item.cb_rect);
                    break;
                case XBinder_ValueType.Object:
                    item.category.subscribe_object(item.cfg.target_key, item.cb_object);
                    break;
            }
        }
        /// <summary>
        /// 从数据源注销监听。
        /// </summary>
        /// <param name="item">运行时绑定条目</param>
        protected virtual void unsubscribe(bind_item item)
        {
            switch (item.cfg.value_type)
            {
                case XBinder_ValueType.Float:
                    item.category.unsubscribe_float(item.cfg.target_key, item.cb_float);
                    break;
                case XBinder_ValueType.Int:
                    item.category.unsubscribe_int(item.cfg.target_key, item.cb_int);
                    break;
                case XBinder_ValueType.Bool:
                    item.category.unsubscribe_bool(item.cfg.target_key, item.cb_bool);
                    break;
                case XBinder_ValueType.String:
                    item.category.unsubscribe_string(item.cfg.target_key, item.cb_string);
                    break;
                case XBinder_ValueType.Color:
                    item.category.unsubscribe_color(item.cfg.target_key, item.cb_color);
                    break;
                case XBinder_ValueType.Vector2:
                    item.category.unsubscribe_vector2(item.cfg.target_key, item.cb_vector2);
                    break;
                case XBinder_ValueType.Vector3:
                    item.category.unsubscribe_vector3(item.cfg.target_key, item.cb_vector3);
                    break;
                case XBinder_ValueType.Vector4:
                    item.category.unsubscribe_vector4(item.cfg.target_key, item.cb_vector4);
                    break;
                case XBinder_ValueType.Quaternion:
                    item.category.unsubscribe_quaternion(item.cfg.target_key, item.cb_quaternion);
                    break;
                case XBinder_ValueType.Rect:
                    item.category.unsubscribe_rect(item.cfg.target_key, item.cb_rect);
                    break;
                case XBinder_ValueType.Object:
                    item.category.unsubscribe_object(item.cfg.target_key, item.cb_object);
                    break;
            }
        }
        #endregion

        /// <summary>
        /// 处理总线数据源重生事件。
        /// <para/>
        /// 遍历所有未绑定的条目，若其目标容器与重生容器匹配，则立即绑定。
        /// </summary>
        /// <param name="created_name">被创建的数据源全名（带前缀）</param>
        protected virtual void handle_category_created(string created_name)
        {
            for (int i = 0; i < bind_items.Count; i++)
            {
                var item = bind_items[i];
                if (item.bound) continue;

                string tar_category = !string.IsNullOrEmpty(item.cfg.target_category) ? item.cfg.target_category : global_category;

                // 修复：比较时拼上前缀，或者直接判断当前总线里有没有这个 category 了
                if (!string.IsNullOrEmpty(tar_category) && created_name == $"xbinder_{tar_category}")
                {
                    bind(item);
                }
            }
        }
        /// <summary>
        /// 当数据源从总线中卸载时。
        /// <para/>
        /// 遍历所有已绑定的条目，若其绑定的数据源正是被销毁的那个，则局部解绑，等待重连。
        /// </summary>
        /// <param name="name">被销毁的数据源名称（带前缀）</param>
        protected virtual void handle_category_destroyed(string name)
        {
            // 遍历所有绑定条目，找到属于被销毁数据源的那个
            for (int i = 0; i < bind_items.Count; i++)
            {
                var item = bind_items[i];

                // 如果这个条目已经绑定了，并且它的 category 的 display_name 就是被销毁的那个
                if (item.bound && item.category != null && item.category.display_name == name)
                {
                    // 1. 内部静默解绑，清空 category 引用
                    unsubscribe(item);
                    item.bound = false;
                    item.category = null;

                    // 2. 子类如果有特殊 UI 表现可以在这里触发（例如先变灰）
                    // on_item_disconnected?.Invoke(item); // 如果有需要的话
                }
            }
        }
    }
}