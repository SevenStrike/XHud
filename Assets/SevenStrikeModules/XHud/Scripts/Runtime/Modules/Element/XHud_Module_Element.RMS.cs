namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections.Generic;
    using UnityEngine;

    public partial class XHud_Module_Element : MonoBehaviour
    {
        [SerializeField]
        /// <summary>
        /// 布局匹配方案是否开启
        /// </summary>
        public bool RMS_Enabled;
        [SerializeField]
        /// <summary>
        ///  布局匹配方案列表
        /// </summary>
        public List<Element_RMS_LayoutData> RMS_LayoutDatas;
        [SerializeField]
        /// <summary>
        ///  布局匹配方案当前选择的标识名称
        /// </summary>
        public string RMS_Name;

        /// <summary>
        /// 根据方案名称获取RMS节点参数
        /// </summary>
        /// <param name="solution"></param>
        public Element_RMS_LayoutData elelemt_RMS_Get(string solution)
        {
            Element_RMS_LayoutData info = null;
            for (int i = 0; i < RMS_LayoutDatas.Count; i++)
            {
                if (solution == RMS_LayoutDatas[i].LayoutName)
                {
                    info = RMS_LayoutDatas[i];
                    break;
                }
            }
            return info;
        }
        /// <summary>
        /// Hud元素 - 是否开启设计布局模式
        /// </summary>
        public void element_RMS_Mode_Enabled(bool state)
        {
            RMS_Enabled = state;
        }
        /// <summary>
        /// 用于响应屏幕分辨率变化时根据元素自身记录的 RMS 布局方案（RMS_Name）重新初始化元素的位置、锚点、缩放等
        /// </summary>
        /// <param name="Indicator"></param>
        /// <param name="Res"></param>
        private void elelemt_RMS_ScreenResolutionChanged(string Indicator, Vector2 Res)
        {
            // 只有开启了 RMS 才处理
            if (!RMS_Enabled)
                return;

            // 检查是否有对应的布局方案
            if (string.IsNullOrEmpty(RMS_Name))
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "RMS已开启但未指定布局方案名称", HudMsgState.警告);
                return;
            }

            XHud_Manager.Instance.hm_HudElement_Initialize_ByDesignLayout_For_Screen(this, Alpha, Vector3.zero, RMS_Name, true);
        }
    }
}