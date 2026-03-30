namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections;
    using UnityEngine;

    public class XHud_Element_Sounder : MonoBehaviour
    {
        public XHud_Module_Element Element;
        public XHud_Module_Toggle Toggle;
        public XHud_Module_Slider Slider;
        public XHud_Module_Progress Progress;
        public XHud_Module_Option Option;
        public XHud_Module_Button Button;
        public string SoundName;
        public float DelayTime;
        public string Indicator;
        public string Timings = "无";

        public bool IsLoop;
        [Range(0, 1)]
        public float Volume = 1f;
        public bool UseRandomPitch;
        public float Pitch_Min = 1f;
        public float Pitch_Max = 1f;
        public bool DebugState;

        public KeyCode PreviewKey;
        public KeyCode UnPreviewKey;

        void Start()
        {

        }

        private void OnEnable()
        {
            if (Element == null)
            {
                Element = GetComponent<XHud_Module_Element>();
            }
            if (Toggle == null)
            {
                Toggle = GetComponent<XHud_Module_Toggle>();
            }
            if (Slider == null)
            {
                Slider = GetComponent<XHud_Module_Slider>();
            }
            if (Progress == null)
            {
                Progress = GetComponent<XHud_Module_Progress>();
            }
            if (Option == null)
            {
                Option = GetComponent<XHud_Module_Option>();
            }
            if (Button == null)
            {
                Button = GetComponent<XHud_Module_Button>();
            }

            RegisterAction();
        }

        private void OnDisable()
        {
            UnRegisterAction();
        }

        public void RegisterAction()
        {
            if (Element != null)
            {
                Element.act_on_element_in_start += ele_act_on_element_in_start;
                Element.act_on_element_in_end += ele_act_on_element_in_end;
                Element.act_on_element_out_start += ele_act_on_element_out_start;
                if (DebugState)
                {
                    XHud_Utilitys.Func_PrintInfo("音效器通知", "已注册 - 元素 - 事件", HudMsgState.通知);
                }
            }
            if (Slider != null)
            {
                Slider.act_on_Press += sli_act_on_Press;
                Slider.act_on_Released += sli_act_on_Released;
                Slider.act_on_ValueChanged += sli_act_on_ValueChanged;
                if (DebugState)
                {
                    XHud_Utilitys.Func_PrintInfo("音效器通知", "已注册 - 滑动条控件 - 事件", HudMsgState.通知);
                }
            }
            if (Toggle != null)
            {
                Toggle.act_on_Press += tog_act_on_Press;
                Toggle.act_on_Released += tog_act_on_Released;
                Toggle.act_on_ValueChanged += tog_act_on_ValueChanged;
                Toggle.act_on_Checked += tog_act_on_Checked;
                Toggle.act_on_UnChecked += tog_act_on_UnChecked;
                if (DebugState)
                {
                    XHud_Utilitys.Func_PrintInfo("音效器通知", "已注册 - 开关控件 - 事件", HudMsgState.通知);
                }
            }
            if (Progress != null)
            {
                Progress.act_on_ValueStart += pro_act_on_ValueStart;
                Progress.act_on_ValueEnd += pro_act_on_ValueEnd;
                Progress.act_on_ValueChanged += pro_act_on_ValueChanged;
                if (DebugState)
                {
                    XHud_Utilitys.Func_PrintInfo("音效器通知", "已注册 - 进度条控件 - 事件", HudMsgState.通知);
                }
            }
            if (Option != null)
            {
                Option.act_on_selector_position_started += opt_act_on_selector_position_started;
                Option.act_on_selector_position_complete += opt_act_on_selector_position_complete;
                Option.act_on_selector_position_changed += opt_act_on_selector_position_changed;
                Option.act_on_option_clicked += opt_act_on_option_clicked;
                if (DebugState)
                {
                    XHud_Utilitys.Func_PrintInfo("音效器通知", "已注册 - 选项器控件 - 事件", HudMsgState.通知);
                }
            }
            if (Button != null)
            {
                Button.act_on_Clicked += btn_act_on_Clicked;
                Button.act_on_DeSelect += btn_act_on_DeSelect;
                Button.act_on_Enter += btn_act_on_Enter;
                Button.act_on_Exit += btn_act_on_Exit;
                Button.act_on_LongPressed += btn_act_on_LongPressed;
                Button.act_on_Press += btn_act_on_Press;
                Button.act_on_Release += btn_act_on_Release;
                Button.act_on_Select += btn_act_on_Select;
                if (DebugState)
                {
                    XHud_Utilitys.Func_PrintInfo("音效器通知", "已注册 - 按钮控件 - 事件", HudMsgState.通知);
                }
            }
        }

        public void UnRegisterAction()
        {
            if (Element != null)
            {
                Element.act_on_element_in_start -= ele_act_on_element_in_start;
                Element.act_on_element_in_end -= ele_act_on_element_in_end;
                Element.act_on_element_out_start -= ele_act_on_element_out_start;
                if (DebugState)
                {
                    XHud_Utilitys.Func_PrintInfo("音效器通知", "已注销 - 元素 - 事件", HudMsgState.通知);
                }
            }
            if (Slider != null)
            {
                Slider.act_on_Press -= sli_act_on_Press;
                Slider.act_on_Released -= sli_act_on_Released;
                Slider.act_on_ValueChanged -= sli_act_on_ValueChanged;
                if (DebugState)
                {
                    XHud_Utilitys.Func_PrintInfo("音效器通知", "已注销 - 滑动条控件 - 事件", HudMsgState.通知);
                }
            }
            if (Toggle != null)
            {
                Toggle.act_on_Press -= tog_act_on_Press;
                Toggle.act_on_Released -= tog_act_on_Released;
                Toggle.act_on_ValueChanged -= tog_act_on_ValueChanged;
                Toggle.act_on_Checked -= tog_act_on_Checked;
                Toggle.act_on_UnChecked -= tog_act_on_UnChecked;
                if (DebugState)
                {
                    XHud_Utilitys.Func_PrintInfo("音效器通知", "已注销 - 开关控件 - 事件", HudMsgState.通知);
                }
            }
            if (Progress != null)
            {
                Progress.act_on_ValueStart -= pro_act_on_ValueStart;
                Progress.act_on_ValueEnd -= pro_act_on_ValueEnd;
                Progress.act_on_ValueChanged -= pro_act_on_ValueChanged;
                if (DebugState)
                {
                    XHud_Utilitys.Func_PrintInfo("音效器通知", "已注销 - 进度条控件 - 事件", HudMsgState.通知);
                }
            }
            if (Option != null)
            {
                Option.act_on_selector_position_started -= opt_act_on_selector_position_started;
                Option.act_on_selector_position_complete -= opt_act_on_selector_position_complete;
                Option.act_on_selector_position_changed -= opt_act_on_selector_position_changed;
                Option.act_on_option_clicked -= opt_act_on_option_clicked;
                if (DebugState)
                {
                    XHud_Utilitys.Func_PrintInfo("音效器通知", "已注销 - 选项器控件 - 事件", HudMsgState.通知);
                }
            }
            if (Button != null)
            {
                Button.act_on_Clicked -= btn_act_on_Clicked;
                Button.act_on_DeSelect -= btn_act_on_DeSelect;
                Button.act_on_Enter -= btn_act_on_Enter;
                Button.act_on_Exit -= btn_act_on_Exit;
                Button.act_on_LongPressed -= btn_act_on_LongPressed;
                Button.act_on_Press -= btn_act_on_Press;
                Button.act_on_Release -= btn_act_on_Release;
                Button.act_on_Select -= btn_act_on_Select;
                if (DebugState)
                {
                    XHud_Utilitys.Func_PrintInfo("音效器通知", "已注销 - 按钮控件 - 事件", HudMsgState.通知);
                }
            }
        }

        #region Button

        private void btn_act_on_Select(XHud_Module_Button btn, Hud_ButtonAction act)
        {
            if (Timings == "鼠标选中")
                PlaySound();
        }

        private void btn_act_on_Release(XHud_Module_Button btn, Hud_ButtonAction act)
        {
            if (Timings == "鼠标松开")
                PlaySound();
        }

        private void btn_act_on_Press(XHud_Module_Button btn, Hud_ButtonAction act)
        {
            if (Timings == "鼠标按下")
                PlaySound();
        }

        private void btn_act_on_LongPressed(XHud_Module_Button btn, Hud_ButtonAction act)
        {
            if (Timings == "鼠标长按")
                PlaySound();
        }

        private void btn_act_on_Exit(XHud_Module_Button btn, Hud_ButtonAction act)
        {
            if (Timings == "鼠标退出")
                PlaySound();
        }

        private void btn_act_on_Enter(XHud_Module_Button btn, Hud_ButtonAction act)
        {
            if (Timings == "鼠标进入")
                PlaySound();
        }

        private void btn_act_on_DeSelect(XHud_Module_Button btn, Hud_ButtonAction act)
        {
            if (Timings == "鼠标取消选中")
                PlaySound();
        }

        private void btn_act_on_Clicked(XHud_Module_Button btn, Hud_ButtonAction act)
        {
            if (Timings == "鼠标点击")
                PlaySound();
        }

        #endregion

        #region Option
        private void opt_act_on_option_clicked(int index, string itemName, Vector3 position)
        {
            if (Timings == "点击选项")
                PlaySound();
        }

        private void opt_act_on_selector_position_changed(Vector3 position, RectTransform selector)
        {
            if (Timings == "光标位置改变")
                PlaySound();
        }

        private void opt_act_on_selector_position_complete(Vector3 position, RectTransform selector)
        {
            if (Timings == "光标移动结束")
                PlaySound();
        }

        private void opt_act_on_selector_position_started(Vector3 position, RectTransform selector)
        {
            if (Timings == "光标移动开始")
                PlaySound();
        }

        #endregion

        #region Progress
        private void pro_act_on_ValueChanged(float progress)
        {
            if (Timings == "进度变化时")
                PlaySound();
        }

        private void pro_act_on_ValueEnd()
        {
            if (Timings == "进度结束时")
                PlaySound();
        }

        private void pro_act_on_ValueStart()
        {
            if (Timings == "进度开始时")
                PlaySound();
        }

        #endregion

        #region Toggle

        private void tog_act_on_UnChecked()
        {
            if (Timings == "开关关闭时")
                PlaySound();
        }

        private void tog_act_on_Checked()
        {
            if (Timings == "开关打开时")
                PlaySound();
        }

        private void tog_act_on_ValueChanged()
        {
            if (Timings == "开关变化时")
                PlaySound();
        }

        private void tog_act_on_Released()
        {
            if (Timings == "开关抬起时")
                PlaySound();
        }

        private void tog_act_on_Press()
        {
            if (Timings == "开关按下时")
                PlaySound();
        }

        #endregion

        #region Slider

        private void sli_act_on_Released(float value)
        {
            if (Timings == "松开滑动条")
                PlaySound();
        }

        private void sli_act_on_ValueChanged(float value)
        {
            if (Timings == "滑动条数值改变")
                PlaySound();
        }

        private void sli_act_on_Press(float value)
        {
            if (Timings == "按下滑动条")
                PlaySound();
        }

        #endregion

        #region Element

        private void ele_act_on_element_out_start(XHud_Module_Element ele)
        {
            if (Timings == "元素退出时")
                PlaySound();
        }

        private void ele_act_on_element_in_end(XHud_Module_Element ele)
        {
            if (Timings == "元素进入后")
                PlaySound();
        }

        private void ele_act_on_element_in_start(XHud_Module_Element ele)
        {
            if (Timings == "元素进入时")
                PlaySound();
        }

        #endregion

        void Update()
        {
            if (Input.GetKeyDown(PreviewKey))
            {
                PlaySound(IsLoop);
            }
            if (Input.GetKeyDown(UnPreviewKey))
            {
                StopSound();
            }
        }

        /// <summary>
        /// 设置循环模式
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void SetLoop(bool state)
        {
            IsLoop = state;
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        public void PlaySound(bool loop = false)
        {
            SetLoop(loop);
            StartCoroutine(DelayPlay());
        }

        /// <summary>
        /// 设置音效信息
        /// </summary>
        /// <param tweenName="soundname">目标音效名称</param>
        public void SetSoundInfo(string soundname)
        {
            SoundName = soundname;
        }

        /// <summary>
        /// 停止音效播放
        /// </summary>
        public void StopSound()
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// 延迟播放
        /// </summary>
        /// <returns></returns>
        IEnumerator DelayPlay()
        {
            if (IsLoop)
            {
                while (true)
                {
                    yield return new WaitForSeconds(DelayTime);
                    ///-------获取播放器
                    AudioSource player = XHud_Manager.Instance.hm_LibrarySounds_GetSounder();
                    ///-------获取声音剪辑
                    if (player != null)
                    {
                        player.clip = XHud_Manager.Instance.Hud_Sounds.SoundLibrary_GetSound(SoundName);
                        player.volume = Volume * XHud_Manager.Instance.Volume;
                        player.mute = XHud_Manager.Instance.VolumeMute;
                        if (UseRandomPitch)
                        {
                            player.pitch = Random.Range(Pitch_Min, Pitch_Max);
                        }
                        else
                        {
                            player.pitch = 1.0f;
                        }
                        player.Play();
                    }
                }
            }
            else
            {
                yield return new WaitForSeconds(DelayTime);
                ///-------获取播放器
                AudioSource player = XHud_Manager.Instance.hm_LibrarySounds_GetSounder();
                ///-------获取声音剪辑
                if (player != null)
                {
                    player.clip = XHud_Manager.Instance.Hud_Sounds.SoundLibrary_GetSound(SoundName);
                    player.volume = Volume * XHud_Manager.Instance.Volume;
                    player.mute = XHud_Manager.Instance.VolumeMute;
                    if (UseRandomPitch)
                    {
                        player.pitch = Random.Range(Pitch_Min, Pitch_Max);
                    }
                    else
                    {
                        player.pitch = 1.0f;
                    }
                    player.Play();
                }
            }
        }


    }
}