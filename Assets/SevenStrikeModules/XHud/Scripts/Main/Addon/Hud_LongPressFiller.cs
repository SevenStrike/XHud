namespace SevenStrikeModules.XHud.Hud
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class Hud_LongPressFiller : MonoBehaviour
    {
        public Hud_Button Button;
        public Image Image;

        void Start()
        {
            Button.act_on_LongPressPer += LongPressed_TimePer;
        }

        private void LongPressed_TimePer(Hud_Button arg0, float arg1)
        {
            Image.fillAmount = arg1;
        }

        void Update()
        {

        }
    }
}