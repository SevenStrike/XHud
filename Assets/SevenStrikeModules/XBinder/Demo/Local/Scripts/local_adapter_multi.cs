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
namespace SevenStrikeModules.XBinder.Example
{
    using SevenStrikeModules.XBinder;
    using UnityEngine.UI;
    using UnityEngine;

    /// <summary>
    /// 示例：本地系统状态面板的多路适配器。
    /// <para/>
    /// 同时绑定 float 与 string 两类数据，分别路由到冷却值、风扇转速以及各类硬件状态文本。
    /// </summary>
    public class local_adapter_multi : XBinder_Adapter_Multi
    {
        #region 字段        
        public Image img_raw;
        public Image img_gra;
        public Text text_title;
        public Text text_sub;
        public Text text_cooling;
        public Text text_cpu;
        public Text text_keyboard;
        public Text text_memory;
        public Text text_monitor;
        public Text text_mouse;
        public Text text_wifi;
        public Text text_fan_title;
        public Text text_speed;
        public Text text_voltage;
        public local_fan_rotator rotator;
        public local_fan_rotator rotator_shadow;
        #endregion

        #region 生命周期
        private void Awake()
        {
            target_category = GetComponentInParent<local_source>().identifier;
        }
        /// <summary>
        /// 组件启用时注册事件并启动基类绑定。
        /// <para/>
        /// 注意：必须在调用 base.OnEnable() 之前注册事件，
        /// 否则基类在绑定成功后派发的初始值回显会被丢弃。
        /// </summary>
        protected override void OnEnable()
        {
            // 必须先注册自己的监听，否则 base 里的初始值回显会丢失
            on_float += handle_float;
            on_string += handle_string;
            on_color += handle_color;
            base.OnEnable();
        }

        /// <summary>
        /// 组件禁用时撤销事件并通知基类解绑。
        /// </summary>
        protected override void OnDisable()
        {
            // 先撤销自己的监听，再让基类去销毁底层
            on_float -= handle_float;
            on_string -= handle_string;
            on_color -= handle_color;
            base.OnDisable();
        }
        #endregion

        #region 事件处理
        /// <summary>
        /// 处理传入的 color 数据。
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="v">颜色值</param>
        private void handle_color(string key, Color v)
        {
            switch (key)
            {
                case "theme_primary_color":
                    img_raw.color = v;
                    img_gra.color = v;
                    break;
            }
        }
        /// <summary>
        /// 处理传入的 float 数据。
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="v">浮点值（如冷却值、风扇转速）</param>
        void handle_float(string key, float v)
        {
            switch (key)
            {
                case "cooling":
                    text_cooling.text = v.ToString();
                    break;
                case "speed":
                    text_speed.text = v.ToString();
                    rotator.set_speed(v);
                    rotator_shadow.set_speed(v);
                    break;
            }
        }

        /// <summary>
        /// 处理传入的 string 数据。
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="v">字符串值（如标题、硬件状态）</param>
        void handle_string(string key, string v)
        {
            switch (key)
            {
                case "title":
                    text_title.text = v;
                    break;
                case "sub":
                    text_sub.text = v;
                    break;
                case "cpu":
                    text_cpu.text = v;
                    break;
                case "keyboard":
                    text_keyboard.text = v;
                    break;
                case "memory":
                    text_memory.text = v;
                    break;
                case "monitor":
                    text_monitor.text = v;
                    break;
                case "mouse":
                    text_mouse.text = v;
                    break;
                case "wifi":
                    text_wifi.text = v;
                    break;
                case "speed":
                    text_speed.text = v;
                    break;
                case "fan_title":
                    text_fan_title.text = v;
                    break;
                case "voltage":
                    text_voltage.text = v;
                    break;
            }
        }
        #endregion

        #region 断线/重连表现
        /// <summary>
        /// 当数据源重新连接时触发。
        /// <para/>
        /// 此时基类已完成重连逻辑，业务层可在此恢复 UI 表现。
        /// </summary>
        /// <param name="name">数据源名称</param>
        protected override void handle_category_created(string name)
        {
            base.handle_category_created(name);
        }
        /// <summary>
        /// 当数据源断开时触发。
        /// <para/>
        /// 此时基类已完成解绑逻辑，业务层应在此告知用户当前处于离线状态。
        /// </summary>
        /// <param name="name">数据源名称</param>
        protected override void handle_category_destroyed(string name)
        {
            base.handle_category_destroyed(name);
        }
        #endregion
    }
}