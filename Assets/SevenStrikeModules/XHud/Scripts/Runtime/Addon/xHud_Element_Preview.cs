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
    using UnityEngine.Events;

    public class XHud_Element_Preview : MonoBehaviour
    {
        #region 字段 - 元素数据
        /// <summary>
        /// 元素模块组件引用
        /// </summary>
        [SerializeField]
        public XHud_Module_Element element;
        /// <summary>
        /// 是否启用预览功能
        /// </summary>
        public bool is_enabled = true;
        /// <summary>
        /// 是否输出调试信息
        /// </summary>
        public bool debug_state = false;
        #endregion

        #region 字段 - 预览按键
        /// <summary>
        /// 元素入场预览按键
        /// </summary>
        public KeyCode key_element_in = KeyCode.C;
        /// <summary>
        /// 元素出场预览按键
        /// </summary>
        public KeyCode key_element_out = KeyCode.V;
        #endregion

        #region 字段 - 动效参数
        /// <summary>
        /// 生成（入场）动效参数
        /// </summary>
        public Motion_Creator motion_create_args;
        /// <summary>
        /// 回收（出场）动效参数
        /// </summary>
        public Motion_Recycler motion_recycle_args;
        /// <summary>
        /// 动效库中生成动效的索引名称
        /// </summary>
        public string motion_create_library_index_name;
        /// <summary>
        /// 动效库中回收动效的索引名称
        /// </summary>
        public string motion_recycle_library_index_name;
        /// <summary>
        /// 入场延时时间
        /// </summary>
        public float delay_with_in;
        /// <summary>
        /// 出场延时时间
        /// </summary>
        public float delay_with_out;
        #endregion

        #region 字段 - 预设变换
        /// <summary>
        /// 预设位置
        /// </summary>
        public Vector3 preset_position;
        /// <summary>
        /// 预设角度
        /// </summary>
        public Vector3 preset_euler;
        /// <summary>
        /// 缩放耗时系数
        /// </summary>
        public float duration_scaler = 1;
        #endregion

        #region 字段 - 显示与 RMS
        /// <summary>
        /// 开始时是否隐藏
        /// </summary>
        public bool hide_with_start = true;
        /// <summary>
        /// 是否启用 RMS 布局
        /// </summary>
        public bool rms_enabled = false;
        /// <summary>
        /// RMS 方案名称
        /// </summary>
        public string rms_name;
        /// <summary>
        /// 元素当前是否处于入场状态
        /// </summary>
        public bool element_is_in = true;
        #endregion

        #region 字段 - 预览运行状态
        /// <summary>
        /// 预览运行状态（序列化私有字段，对外通过 PreviewIsRunning 属性访问）
        /// </summary>
        [SerializeField]
        private bool m_preview_is_running;
        /// <summary>
        /// 预览运行状态字段名常量，供编辑器脚本以 nameof 方式查找序列化属性
        /// </summary>
        public const string editor_preview_is_running = nameof(m_preview_is_running);
        /// <summary>
        /// 预览运行状态属性：值变化时触发 act_on_element_preview_statechanged 回调
        /// </summary>
        public bool preview_is_running
        {
            get
            {
                return m_preview_is_running;
            }
            set
            {
                if (value != m_preview_is_running)
                {
                    m_preview_is_running = value;

                    if (act_on_element_preview_statechanged != null)
                        act_on_element_preview_statechanged(value);
                }
            }
        }
        #endregion

        #region 字段 - 事件回调
        /// <summary>
        /// 动作 - 元素 入场 - 开始
        /// </summary>
        public UnityAction act_on_element_preview_in_start;
        /// <summary>
        /// 动作 - 元素 入场 - 结束
        /// </summary>
        public UnityAction act_on_element_preview_in_end;
        /// <summary>
        /// 动作 - 元素 出场 - 开始
        /// </summary>
        public UnityAction act_on_element_preview_out_start;
        /// <summary>
        /// 动作 - 元素 出场 - 结束
        /// </summary>
        public UnityAction act_on_element_preview_out_end;
        /// <summary>
        /// 动作 - 元素状态改变
        /// </summary>
        public UnityAction<bool> act_on_element_preview_statechanged;
        #endregion

        #region 折叠
        public bool
          fold_options = true,
          fold_param = true,
          fold_key = true,
          fold_state = true,
          fold_motion = true,
          fold_preset_trans = true,
          fold_rms = true,
          fold_based = true;

        public void GroupFold(bool state)
        {
            fold_options = state;
            fold_param = state;
            fold_key = state;
            fold_state = state;
            fold_motion = state;
            fold_preset_trans = state;
            fold_rms = state;
            fold_based = state;
        }

        public void GroupFold(bool options = true, bool param = true, bool key = true, bool state = true, bool motion = true, bool preset_trans = true, bool rms = true, bool based = true)
        {
            fold_options = options;
            fold_param = param;
            fold_key = key;
            fold_state = state;
            fold_motion = motion;
            fold_preset_trans = preset_trans;
            fold_rms = rms;
            fold_based = based;
        }
        #endregion

        private void Awake()
        {
            if (element == null)
                element = GetComponent<XHud_Module_Element>();

            // 原始姿态数据收集
            OriginalDataCollect();
        }

        private void Start()
        {
            if (hide_with_start)
            {
                element_is_in = false;
                element.element_AlphaSet(0);
                element.element_Reset(false, false, false);
                if (debug_state)
                    XGUI_Utilitys.Console("XHud - 元素预览器通知", "预览前初始化元素透明度为0！ ", XGUIMsgState.通知);
            }
            else
            {
                element_is_in = true;
            }
        }

        private void Update()
        {
            ControlUpdate();
        }

        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        /// <summary>
        /// 收集原始姿态数据
        /// </summary>
        public void OriginalDataCollect()
        {
            // 原始姿态数据收集：获取位置
            preset_position = element.rect_transform.anchoredPosition3D;
            // 原始姿态数据收集：获取角度
            preset_euler = element.rect_transform.localEulerAngles;
        }

        private void ControlUpdate()
        {
            if (Input.GetKeyDown(key_element_in))
            {
                Preview_Element_In();
            }

            if (Input.GetKeyDown(key_element_out))
            {
                Preview_Element_Out();
            }

            if (is_enabled)
                element.primitives_tween_global_duration = duration_scaler;
        }

        /// <summary>
        /// 预览Hud元素进入效果
        /// </summary>
        public void Preview_Element_In()
        {
            if (preview_is_running)
                return;
            if (element_is_in)
                return;
            preview_is_running = true;

            element_is_in = true;

            if (!is_enabled)
            {
                if (debug_state)
                    XGUI_Utilitys.Console("XHud - 元素预览器通知", "预览开关已关闭！ ", XGUIMsgState.错误);
                return;
            }
            if (element.animate_state == XHudElementAnimateState.Animating)
                return;
            StartCoroutine(Preview_Delay_In());
            if (debug_state)
                XGUI_Utilitys.Console("XHud - 元素预览器通知", "预览 - 元素进入！ ", XGUIMsgState.通知);
        }

        /// <summary>
        /// 预览Hud元素退出效果
        /// </summary>
        public void Preview_Element_Out()
        {
            if (preview_is_running)
                return;
            if (!element_is_in)
                return;
            preview_is_running = true;

            element_is_in = false;

            if (!is_enabled)
            {
                if (debug_state)
                    XGUI_Utilitys.Console("XHud - 元素预览器通知", "预览开关已关闭！ ", XGUIMsgState.错误);
                return;
            }
            if (element.animate_state == XHudElementAnimateState.Animating)
                return;
            StartCoroutine(Preview_Delay_Out());
            if (debug_state)
                XGUI_Utilitys.Console("XHud - 元素预览器通知", "预览 - 元素退出！ ", XGUIMsgState.通知);
        }

        IEnumerator Preview_Delay_In()
        {
            yield return new WaitForSeconds(delay_with_in);

            if (act_on_element_preview_in_start != null)
                act_on_element_preview_in_start();

            // 进入前强制透明度 0
            element.element_AlphaSet(0);

            // 如果 RMS 开启，进入前先强制设置变换信息：根据 RMS 信息获取
            if (rms_enabled && element.rms_enabled)
            {
                for (int i = 0; i < element.rms_layout_datas.Count; i++)
                {
                    if (element.rms_layout_datas[i].layout_name == rms_name)
                    {
                        XHud_Manager.instance.hm_ScreenElement_Initialize_By_RMS(element, rms_name, true);
                        break;
                    }
                }
            }
            else
            {
                // 进入前先强制设置位置：原始位置
                element.element_PositionSet(preset_position);
                // 进入前强制设置角度：原始角度
                element.element_RotationSet(preset_euler);
            }

            // 保险操作：倒退动画
            element.PrimitiveTweens_Rewind();
            // 开始进入
            element.Element_In(motion_create_args, () =>
            {
                preview_is_running = false;

                if (act_on_element_preview_in_end != null)
                    act_on_element_preview_in_end();
            });
        }

        IEnumerator Preview_Delay_Out()
        {
            yield return new WaitForSeconds(delay_with_out);

            if (act_on_element_preview_out_start != null)
                act_on_element_preview_out_start();

            element.Element_Out(motion_recycle_args, () =>
            {
                preview_is_running = false;

                if (act_on_element_preview_out_end != null)
                    act_on_element_preview_out_end();
            });
        }
    }
}