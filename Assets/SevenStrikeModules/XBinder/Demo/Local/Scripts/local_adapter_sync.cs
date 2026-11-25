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
    using UnityEngine;
    using UnityEngine.UI;

    public class local_adapter_sync : MonoBehaviour
    {
        public XBinder_Adapter_Sync_Float adapter;
        public Slider slider;

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

        public void Awake()
        {
            adapter.target_category = GetComponentInParent<XBinder_Source>().identifier;
        }

        void OnEnable()
        {
            adapter.act_on_value_changed += on_data_changed;
            adapter.act_handle_category_created += Adapter_act_on_category_created;
            adapter.act_handle_category_destroyed += Adapter_act_on_category_destroyed;
            slider.onValueChanged.AddListener(slider_value_changed);
        }

        private void OnDisable()
        {
            adapter.act_on_value_changed -= on_data_changed;
            adapter.act_handle_category_created -= Adapter_act_on_category_created;
            adapter.act_handle_category_destroyed -= Adapter_act_on_category_destroyed;
            slider.onValueChanged.RemoveListener(slider_value_changed);
        }

        private void slider_value_changed(float value)
        {
            adapter.push(value);
        }

        private void on_data_changed(float value)
        {
            slider.SetValueWithoutNotify(value);
        }

        #region 断线/重连表现
        private void Adapter_act_on_category_created(string name)
        {

        }

        private void Adapter_act_on_category_destroyed(string name)
        {

        }
        #endregion
    }
}