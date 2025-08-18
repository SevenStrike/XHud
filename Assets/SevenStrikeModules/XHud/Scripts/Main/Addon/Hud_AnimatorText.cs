using SevenStrikeModules.XHud.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud_AnimatorText : MonoBehaviour
{
    public Hud_Animator Animator;
    public Text Text;
    public Image progress;

    void Start()
    {
        Animator.TweenNode_GetByIndicator("CustomValue_Float").Act_On_Float_Changed += Change;
        Animator.TweenNode_GetByIndicator("CustomValue_Float").Act_On_Float_Rewind += Rewind;
    }

    private void Rewind(float val)
    {
        SetText(val.ToString("F2"));
        progress.fillAmount = val / 100;
    }

    private void Change(float val)
    {
        SetText(val.ToString("F2"));
        progress.fillAmount = val / 100;
    }

    private void SetText(string text)
    {
        Text.text = text;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
