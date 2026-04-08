using UnityEngine;

namespace SevenStrikeModules.XHud
{
    public class sdd : MonoBehaviour
    {
        public XHud_Module_Progress progress;
        public float value;

        void Start()
        {
            progress = GetComponent<XHud_Module_Progress>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                if (value >= 1)
                {
                    value = 1;
                }
                else
                    value += 0.5f;

                progress.pro_SetProgressValue(value);
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                value = 0;
                progress.pro_ProgressValueReset();
            }
        }
    }
}
