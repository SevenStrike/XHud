namespace SevenStrikeModules.XHud
{
    using UnityEngine;
    using UnityEngine.UI;

    public class XHud_PerformanceMonitor : MonoBehaviour
    {
        [SerializeField] public float updateInterval = 0.4f;
        [SerializeField] private float lastUpdateTime = 0f;

        /// <summary>
        /// 节点内容边距
        /// </summary>
        [SerializeField] public float RectMargin = 20;
        /// <summary>
        /// 节点内容比例
        /// </summary>
        [SerializeField] public float ScaleRatio = 1;
        /// <summary>
        /// 标题
        /// </summary>
        [SerializeField] public XHud_Module_TmpText Title;
        /// <summary>
        /// 滚动条
        /// </summary>
        [SerializeField] public ScrollRect ScrollRect;
        /// <summary>
        /// 数据节点列表
        /// </summary>
        public xHud_MonitorData[] MonitorDataNodes;

        private void Start()
        {
            GetAllMonitorDataNodes();
            lastUpdateTime = Time.time;
        }
        private void Update()
        {
            // 每隔 updateInterval 秒更新一次数据
            if (Time.time - lastUpdateTime >= updateInterval)
            {
                for (int i = 0; i < MonitorDataNodes.Length; i++)
                {
                    MonitorDataNodes[i].UpdateData();
                }
                lastUpdateTime = Time.time;
            }
        }
        /// <summary>
        /// 获取下面所有子物体中的MonitorData节点
        /// </summary>
        private void GetAllMonitorDataNodes()
        {
            MonitorDataNodes = GetComponentsInChildren<xHud_MonitorData>(true);
            for (int i = 0; i < MonitorDataNodes.Length; i++)
            {
                MonitorDataNodes[i].Initialize();
            }
        }

    }
}