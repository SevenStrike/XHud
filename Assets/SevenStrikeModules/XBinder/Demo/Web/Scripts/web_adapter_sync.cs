namespace SevenStrikeModules.XBinder.Example
{
    using SevenStrikeModules.XBinder;
    using UnityEngine;
    using UnityEngine.UI;

    public class web_adapter_sync : MonoBehaviour
    {
        public XBinder_Adapter_Sync_Float adapter;
        public Slider slider;

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