using UnityEngine;

namespace SevenStrikeModules.XGUI.Runtime
{
    [System.Serializable]
    public class XGUI_CardData
    {
        [SerializeField]
        public string _title;
        [SerializeField]
        public object _data;
        [SerializeField]
        public Color _bgcolor;
    }

    [System.Serializable]
    public class XGUI_CardDataWrap
    {
        [SerializeField]
        public XGUI_CardData[] _datas;
        [SerializeField]
        public int index;
    }
}
