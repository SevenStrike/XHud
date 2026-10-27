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
        #region 字段 - 关联组件
        /// <summary>
        /// 关联元素组件
        /// </summary>
        [SerializeField]
        public XHud_Module_Element element;
        /// <summary>
        /// 关联开关组件
        /// </summary>
        [SerializeField]
        public XHud_Module_Toggle toggle;
        /// <summary>
        /// 关联滑动条组件
        /// </summary>
        [SerializeField]
        public XHud_Module_Slider slider;
        /// <summary>
        /// 关联进度条组件
        /// </summary>
        [SerializeField]
        public XHud_Module_Progress progress;
        /// <summary>
        /// 关联选项组件
        /// </summary>
        [SerializeField]
        public XHud_Module_Option option;
        /// <summary>
        /// 关联按钮组件
        /// </summary>
        [SerializeField]
        public XHud_Module_Button button;
        #endregion

        #region 字段 - 音效基础信息
        /// <summary>
        /// 音效名称
        /// </summary>
        [SerializeField]
        public string sound_name;
        /// <summary>
        /// 延迟播放时间
        /// </summary>
        [SerializeField]
        public float delay_time;
        /// <summary>
        /// 音效标识
        /// </summary>
        [SerializeField]
        public string indicator;
        /// <summary>
        /// 播放时机
        /// </summary>
        [SerializeField]
        public string timings = "无";
        #endregion

        #region 字段 - 音量与音高
        /// <summary>
        /// 是否循环播放
        /// </summary>
        [SerializeField]
        public bool is_loop;
        /// <summary>
        /// 音量（0 到 1）
        /// </summary>
        [SerializeField]
        [Range(0, 1)]
        public float volume = 1f;
        /// <summary>
        /// 是否使用随机音高
        /// </summary>
        [SerializeField]
        public bool use_random_pitch;
        /// <summary>
        /// 随机音高下限
        /// </summary>
        [SerializeField]
        public float pitch_min = 1f;
        /// <summary>
        /// 随机音高上限
        /// </summary>
        [SerializeField]
        public float pitch_max = 1f;
        #endregion

        #region 字段 - 调试与预览
        /// <summary>
        /// 是否输出调试信息
        /// </summary>
        [SerializeField]
        public bool debug_state;
        /// <summary>
        /// 预览快捷键
        /// </summary>
        [SerializeField]
        public KeyCode preview_key;
        /// <summary>
        /// 取消预览快捷键
        /// </summary>
        [SerializeField]
        public KeyCode un_preview_key;
        #endregion

        #region 折叠
        public bool
            fold_param = true,
            fold_option = true,
            fold_state = true,
            fold_based = true;

        public void GroupFold(bool state)
        {
            fold_param = state;
            fold_option = state;
            fold_state = state;
            fold_based = state;
        }

        public void GroupFold(bool param = true, bool option = true, bool state = true, bool based = true)
        {
            fold_param = param;
            fold_option = option;
            fold_state = state;
            fold_based = based;
        }
        #endregion

        void Start()
        {

        }

        private void OnEnable()
        {
            if (element == null)
            {
                element = GetComponent<XHud_Module_Element>();
            }
            if (toggle == null)
            {
                toggle = GetComponent<XHud_Module_Toggle>();
            }
            if (slider == null)
            {
                slider = GetComponent<XHud_Module_Slider>();
            }
            if (progress == null)
            {
                progress = GetComponent<XHud_Module_Progress>();
            }
            if (option == null)
            {
                option = GetComponent<XHud_Module_Option>();
            }
            if (button == null)
            {
                button = GetComponent<XHud_Module_Button>();
            }

            RegisterAction();
        }

        private void OnDisable()
        {
            UnRegisterAction();
        }

        public void RegisterAction()
        {
            if (element != null)
            {
                element.act_on_element_in_start += ele_act_on_element_in_start;
                element.act_on_element_in_end += ele_act_on_element_in_end;
                element.act_on_element_out_start += ele_act_on_element_out_start;
                if (debug_state)
                {
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注册 - 元素 - 事件", XGUIMsgState.通知);
                }
            }
            if (slider != null)
            {
                slider.act_on_Press += sli_act_on_Press;
                slider.act_on_Released += sli_act_on_Released;
                slider.act_on_ValueChanged += sli_act_on_ValueChanged;
                if (debug_state)
                {
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注册 - 滑动条控件 - 事件", XGUIMsgState.通知);
                }
            }
            if (toggle != null)
            {
                toggle.act_on_Press += tog_act_on_Press;
                toggle.act_on_Released += tog_act_on_Released;
                toggle.act_on_ValueChanged += tog_act_on_ValueChanged;
                toggle.act_on_Checked += tog_act_on_Checked;
                toggle.act_on_UnChecked += tog_act_on_UnChecked;
                if (debug_state)
                {
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注册 - 开关控件 - 事件", XGUIMsgState.通知);
                }
            }
            if (progress != null)
            {
                progress.act_on_ValueStart += pro_act_on_ValueStart;
                progress.act_on_ValueEnd += pro_act_on_ValueEnd;
                progress.act_on_ValueChanged += pro_act_on_ValueChanged;
                if (debug_state)
                {
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注册 - 进度条控件 - 事件", XGUIMsgState.通知);
                }
            }
            if (option != null)
            {
                option.act_on_selector_position_started += opt_act_on_selector_position_started;
                option.act_on_selector_position_complete += opt_act_on_selector_position_complete;
                option.act_on_selector_position_changed += opt_act_on_selector_position_changed;
                option.act_on_option_clicked += opt_act_on_option_clicked;
                if (debug_state)
                {
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注册 - 选项器控件 - 事件", XGUIMsgState.通知);
                }
            }
            if (button != null)
            {
                button.act_on_Clicked += btn_act_on_Clicked;
                button.act_on_DeSelect += btn_act_on_DeSelect;
                button.act_on_Enter += btn_act_on_Enter;
                button.act_on_Exit += btn_act_on_Exit;
                button.act_on_LongPressed += btn_act_on_LongPressed;
                button.act_on_Press += btn_act_on_Press;
                button.act_on_Release += btn_act_on_Release;
                button.act_on_Select += btn_act_on_Select;
                if (debug_state)
                {
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注册 - 按钮控件 - 事件", XGUIMsgState.通知);
                }
            }
        }

        public void UnRegisterAction()
        {
            if (element != null)
            {
                element.act_on_element_in_start -= ele_act_on_element_in_start;
                element.act_on_element_in_end -= ele_act_on_element_in_end;
                element.act_on_element_out_start -= ele_act_on_element_out_start;
                if (debug_state)
                {
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注销 - 元素 - 事件", XGUIMsgState.通知);
                }
            }
            if (slider != null)
            {
                slider.act_on_Press -= sli_act_on_Press;
                slider.act_on_Released -= sli_act_on_Released;
                slider.act_on_ValueChanged -= sli_act_on_ValueChanged;
                if (debug_state)
                {
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注销 - 滑动条控件 - 事件", XGUIMsgState.通知);
                }
            }
            if (toggle != null)
            {
                toggle.act_on_Press -= tog_act_on_Press;
                toggle.act_on_Released -= tog_act_on_Released;
                toggle.act_on_ValueChanged -= tog_act_on_ValueChanged;
                toggle.act_on_Checked -= tog_act_on_Checked;
                toggle.act_on_UnChecked -= tog_act_on_UnChecked;
                if (debug_state)
                {
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注销 - 开关控件 - 事件", XGUIMsgState.通知);
                }
            }
            if (progress != null)
            {
                progress.act_on_ValueStart -= pro_act_on_ValueStart;
                progress.act_on_ValueEnd -= pro_act_on_ValueEnd;
                progress.act_on_ValueChanged -= pro_act_on_ValueChanged;
                if (debug_state)
                {
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注销 - 进度条控件 - 事件", XGUIMsgState.通知);
                }
            }
            if (option != null)
            {
                option.act_on_selector_position_started -= opt_act_on_selector_position_started;
                option.act_on_selector_position_complete -= opt_act_on_selector_position_complete;
                option.act_on_selector_position_changed -= opt_act_on_selector_position_changed;
                option.act_on_option_clicked -= opt_act_on_option_clicked;
                if (debug_state)
                {
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注销 - 选项器控件 - 事件", XGUIMsgState.通知);
                }
            }
            if (button != null)
            {
                button.act_on_Clicked -= btn_act_on_Clicked;
                button.act_on_DeSelect -= btn_act_on_DeSelect;
                button.act_on_Enter -= btn_act_on_Enter;
                button.act_on_Exit -= btn_act_on_Exit;
                button.act_on_LongPressed -= btn_act_on_LongPressed;
                button.act_on_Press -= btn_act_on_Press;
                button.act_on_Release -= btn_act_on_Release;
                button.act_on_Select -= btn_act_on_Select;
                if (debug_state)
                {
                    XGUI_Utilitys.Console("XHud - 音效器通知", "已注销 - 按钮控件 - 事件", XGUIMsgState.通知);
                }
            }
        }

        #region Button

        private void btn_act_on_Select(XHud_Module_Button btn, Hud_ButtonAction act)
        {
            if (timings == "鼠标选中")
                PlaySound();
        }

        private void btn_act_on_Release(XHud_Module_Button btn, Hud_ButtonAction act)
        {
            if (timings == "鼠标松开")
                PlaySound();
        }

        private void btn_act_on_Press(XHud_Module_Button btn, Hud_ButtonAction act)
        {
            if (timings == "鼠标按下")
                PlaySound();
        }

        private void btn_act_on_LongPressed(XHud_Module_Button btn, Hud_ButtonAction act)
        {
            if (timings == "鼠标长按")
                PlaySound();
        }

        private void btn_act_on_Exit(XHud_Module_Button btn, Hud_ButtonAction act)
        {
            if (timings == "鼠标退出")
                PlaySound();
        }

        private void btn_act_on_Enter(XHud_Module_Button btn, Hud_ButtonAction act)
        {
            if (timings == "鼠标进入")
                PlaySound();
        }

        private void btn_act_on_DeSelect(XHud_Module_Button btn, Hud_ButtonAction act)
        {
            if (timings == "鼠标取消选中")
                PlaySound();
        }

        private void btn_act_on_Clicked(XHud_Module_Button btn, Hud_ButtonAction act)
        {
            if (timings == "鼠标点击")
                PlaySound();
        }

        #endregion

        #region Option
        private void opt_act_on_option_clicked(int index, string itemName, Vector3 position)
        {
            if (timings == "点击选项")
                PlaySound();
        }

        private void opt_act_on_selector_position_changed(Vector3 position, RectTransform selector)
        {
            if (timings == "光标位置改变")
                PlaySound();
        }

        private void opt_act_on_selector_position_complete(Vector3 position, RectTransform selector)
        {
            if (timings == "光标移动结束")
                PlaySound();
        }

        private void opt_act_on_selector_position_started(Vector3 position, RectTransform selector)
        {
            if (timings == "光标移动开始")
                PlaySound();
        }

        #endregion

        #region Progress
        private void pro_act_on_ValueChanged(float progress)
        {
            if (timings == "进度变化时")
                PlaySound();
        }

        private void pro_act_on_ValueEnd()
        {
            if (timings == "进度结束时")
                PlaySound();
        }

        private void pro_act_on_ValueStart()
        {
            if (timings == "进度开始时")
                PlaySound();
        }

        #endregion

        #region Toggle

        private void tog_act_on_UnChecked()
        {
            if (timings == "开关关闭时")
                PlaySound();
        }

        private void tog_act_on_Checked()
        {
            if (timings == "开关打开时")
                PlaySound();
        }

        private void tog_act_on_ValueChanged()
        {
            if (timings == "开关变化时")
                PlaySound();
        }

        private void tog_act_on_Released()
        {
            if (timings == "开关抬起时")
                PlaySound();
        }

        private void tog_act_on_Press()
        {
            if (timings == "开关按下时")
                PlaySound();
        }

        #endregion

        #region Slider

        private void sli_act_on_Released(float value)
        {
            if (timings == "松开滑动条")
                PlaySound();
        }

        private void sli_act_on_ValueChanged(float value)
        {
            if (timings == "滑动条数值改变")
                PlaySound();
        }

        private void sli_act_on_Press(float value)
        {
            if (timings == "按下滑动条")
                PlaySound();
        }

        #endregion

        #region Element

        private void ele_act_on_element_out_start(XHud_Module_Element ele)
        {
            if (timings == "元素退出时")
                PlaySound();
        }

        private void ele_act_on_element_in_end(XHud_Module_Element ele)
        {
            if (timings == "元素进入后")
                PlaySound();
        }

        private void ele_act_on_element_in_start(XHud_Module_Element ele)
        {
            if (timings == "元素进入时")
                PlaySound();
        }

        #endregion

        void Update()
        {
            if (Input.GetKeyDown(preview_key))
            {
                PlaySound(is_loop);
            }
            if (Input.GetKeyDown(un_preview_key))
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
            is_loop = state;
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
            sound_name = soundname;
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
            if (is_loop)
            {
                while (true)
                {
                    yield return new WaitForSeconds(delay_time);
                    ///-------获取播放器
                    AudioSource player = XHud_Manager.instance.hm_LibrarySounds_GetSounder();
                    ///-------获取声音剪辑
                    if (player != null)
                    {
                        player.clip = XHud_Manager.instance.library_sounds.SoundLibrary_GetSound(sound_name);
                        player.volume = volume * XHud_Manager.instance.global_volume * 0.01f;
                        player.mute = XHud_Manager.instance.global_volume_mute;
                        if (use_random_pitch)
                        {
                            player.pitch = Random.Range(pitch_min, pitch_max);
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
                yield return new WaitForSeconds(delay_time);
                ///-------获取播放器
                AudioSource player = XHud_Manager.instance.hm_LibrarySounds_GetSounder();
                ///-------获取声音剪辑
                if (player != null)
                {
                    player.clip = XHud_Manager.instance.library_sounds.SoundLibrary_GetSound(sound_name);
                    player.volume = volume * XHud_Manager.instance.global_volume * 0.01f;
                    player.mute = XHud_Manager.instance.global_volume_mute;
                    if (use_random_pitch)
                    {
                        player.pitch = Random.Range(pitch_min, pitch_max);
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