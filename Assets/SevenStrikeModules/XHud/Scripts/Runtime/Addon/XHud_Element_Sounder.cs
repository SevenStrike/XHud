/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XHud - Unity UGUI 高级管理架构插件
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
namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XGUI.Runtime;
    using SevenStrikeModules.XHud.Enums;
    using System.Collections;
    using UnityEngine;

    [SerializeField]
    public class XHud_Element_Sounder : MonoBehaviour
    {
        [SerializeField]
        public XHud_Module_Element Element;
        [SerializeField]
        public XHud_Module_Toggle Toggle;
        [SerializeField]
        public XHud_Module_Slider Slider;
        [SerializeField]
        public XHud_Module_Progress Progress;
        [SerializeField]
        public XHud_Module_Option Option;
        [SerializeField]
        public XHud_Module_Button Button;
        [SerializeField]
        public string SoundName;
        [SerializeField]
        public float DelayTime;
        [SerializeField]
        public string Indicator;
        [SerializeField]
        public string Timings = "无";

        [SerializeField]
        public bool IsLoop;
        [SerializeField]
        [Range(0, 1)]
        public float Volume = 1f;
        [SerializeField]
        public bool UseRandomPitch;
        [SerializeField]
        public float Pitch_Min = 1f;
        [SerializeField]
        public float Pitch_Max = 1f;
        [SerializeField]
        public bool DebugState;

        [SerializeField]
        public KeyCode PreviewKey;
        [SerializeField]
        public KeyCode UnPreviewKey;

        public bool
            fold_param = true,
            fold_option = true,
            fold_state = true,
            fold_based = true;

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
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注册 - 元素 - 事件", XGUIMsgState.通知);
                }
            }
            if (Slider != null)
            {
                Slider.act_on_Press += sli_act_on_Press;
                Slider.act_on_Released += sli_act_on_Released;
                Slider.act_on_ValueChanged += sli_act_on_ValueChanged;
                if (DebugState)
                {
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注册 - 滑动条控件 - 事件", XGUIMsgState.通知);
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
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注册 - 开关控件 - 事件", XGUIMsgState.通知);
                }
            }
            if (Progress != null)
            {
                Progress.act_on_ValueStart += pro_act_on_ValueStart;
                Progress.act_on_ValueEnd += pro_act_on_ValueEnd;
                Progress.act_on_ValueChanged += pro_act_on_ValueChanged;
                if (DebugState)
                {
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注册 - 进度条控件 - 事件", XGUIMsgState.通知);
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
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注册 - 选项器控件 - 事件", XGUIMsgState.通知);
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
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注册 - 按钮控件 - 事件", XGUIMsgState.通知);
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
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注销 - 元素 - 事件", XGUIMsgState.通知);
                }
            }
            if (Slider != null)
            {
                Slider.act_on_Press -= sli_act_on_Press;
                Slider.act_on_Released -= sli_act_on_Released;
                Slider.act_on_ValueChanged -= sli_act_on_ValueChanged;
                if (DebugState)
                {
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注销 - 滑动条控件 - 事件", XGUIMsgState.通知);
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
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注销 - 开关控件 - 事件", XGUIMsgState.通知);
                }
            }
            if (Progress != null)
            {
                Progress.act_on_ValueStart -= pro_act_on_ValueStart;
                Progress.act_on_ValueEnd -= pro_act_on_ValueEnd;
                Progress.act_on_ValueChanged -= pro_act_on_ValueChanged;
                if (DebugState)
                {
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注销 - 进度条控件 - 事件", XGUIMsgState.通知);
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
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注销 - 选项器控件 - 事件", XGUIMsgState.通知);
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
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注销 - 按钮控件 - 事件", XGUIMsgState.通知);
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
        /// <param name="treeState"></param>
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
        /// <param name="soundname">目标音效名称</param>
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