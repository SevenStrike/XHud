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
    using System.Collections.Generic;
    using UnityEngine.UI;

    /// <summary>
    /// 示例：Web 数据面板的多路适配器。
    /// <para/>
    /// 同时绑定 string、float、object 三类数据，分别路由到标题、风扇转速、以及循环列表。
    /// </summary>
    public class web_adapter_multi : XBinder_Adapter_Multi
    {
        #region 字段
        public Text text_title;
        public Text text_sub;
        public Text text_mx_title;
        public Text text_mx_sub;
        public Text text_mx_uper;
        public Text text_mx_footer;
        public Text text_fan_title;
        public Text text_fan_speed;
        public Text text_fan_voltage;
        public Text[] mx_titles;
        public Text[] mx_subs;
        public Text[] mx_values;
        public web_sequencer mx_seq;
        public local_fan_rotator rotator;
        public local_fan_rotator rotator_shadow;
        #endregion

        #region 生命周期
        /// <summary>
        /// 组件启用时注册事件并启动基类绑定。
        /// </summary>
        protected override void OnEnable()
        {
            // 必须先注册自己的监听，否则 base 里的初始值回显会丢失
            on_string += handle_string;
            on_float += handle_float;
            on_object += handle_object;
            base.OnEnable();
        }
        /// <summary>
        /// 组件禁用时撤销事件并通知基类解绑。
        /// </summary>
        protected override void OnDisable()
        {
            // 先撤销自己的监听，再让基类去销毁底层
            on_string -= handle_string;
            on_float -= handle_float;
            on_object -= handle_object;
            base.OnDisable();
        }
        #endregion

        #region 事件处理
        /// <summary>
        /// 处理 string 类型的数据推送。
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="v">字符串值</param>
        private void handle_string(string key, string v)
        {
            switch (key)
            {
                case "title": text_title.text = v; break;
                case "sub": text_sub.text = v; break;
                case "mx_title": text_mx_title.text = v; break;
                case "mx_uper": text_mx_uper.text = v; break;
                case "mx_sub": text_mx_sub.text = v; break;
                case "mx_footer": text_mx_footer.text = v; break;
                case "center_icon": mx_seq.seq_type_set(v); break;
                case "fan_title": text_fan_title.text = v; break;
                case "voltage": text_fan_voltage.text = v; break;
            }
        }
        /// <summary>
        /// 处理 float 类型的数据推送。
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="v">浮点值</param>
        private void handle_float(string key, float v)
        {
            switch (key)
            {
                case "speed":
                    rotator.set_speed(v);
                    rotator_shadow.set_speed(v);
                    text_fan_speed.text = v.ToString("F2");
                    break;
            }
        }
        /// <summary>
        /// 处理 object 类型的数据推送（通常用于批量列表）。
        /// </summary>
        /// <param name="key">数据键名</param>
        /// <param name="value">对象值，需强转为具体类型</param>
        private void handle_object(string key, object value)
        {
            switch (key)
            {
                case "data":
                    var datas = value as List<web_Matrix_Loop_Pressure_Relief_Data>;
                    if (datas == null) return;

                    // ✅ 新增：取最小长度，防止数据源数量超过 UI 数组导致越界崩溃
                    int count = System.Math.Min(datas.Count, mx_titles.Length);
                    count = System.Math.Min(count, mx_subs.Length);
                    count = System.Math.Min(count, mx_values.Length);

                    for (int i = 0; i < count; i++)
                    {
                        mx_titles[i].text = datas[i].stellarCycle;
                        mx_subs[i].text = datas[i].sectorName;
                        mx_values[i].text = datas[i].energyDrain;
                    }
                    break;
            }
        }
        #endregion

        #region 断线/重连表现
        /// <summary>
        /// 当数据源重新连接时，恢复 UI 表现。
        /// </summary>
        /// <param name="name">数据源名称</param>
        protected override void handle_category_created(string name)
        {
            base.handle_category_created(name);
        }
        /// <summary>
        /// 当数据源断开时，更新 UI 表现，告知用户当前处于离线状态。
        /// </summary>
        /// <param name="name">数据源名称</param>
        protected override void handle_category_destroyed(string name)
        {
            base.handle_category_destroyed(name);
        }
        #endregion
    }
}