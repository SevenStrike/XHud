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
 */namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;

    public class XHud_Element_Preview : MonoBehaviour
    {
        [SerializeField]
        public XHud_Module_Element HudElement;

        public bool IsEnable = true;
        public bool DebugState = false;

        public KeyCode key_Element_In = KeyCode.C;
        public KeyCode key_Element_Out = KeyCode.V;

        public Motion_Creator CreateArgs;
        public Motion_Recycler RecycleArgs;
        public bool HideWithStart = true;
        public Vector3 OriginalPosition;
        public Vector3 OriginalEuler;
        public float DurationScaler = 1;

        [SerializeField]
        private bool previewIsRunning;
        public bool PreviewIsRunning
        {
            get
            {
                return previewIsRunning;
            }
            set
            {
                if (value != previewIsRunning)
                {
                    previewIsRunning = value;

                    if (act_on_element_preview_statechanged != null)
                        act_on_element_preview_statechanged(value);
                }
            }
        }

        public string Crc_Lib_Name;
        public string Rec_Lib_Name;

        public float DelayWithIn;
        public float DelayWithOut;

        public bool create_fold_move;
        public bool create_fold_rotate;
        public bool create_fold_alpha;
        public bool recycle_fold_move;
        public bool recycle_fold_rotate;
        public bool recycle_fold_alpha;

        public bool RMS_Enabled = false;
        public string RMS_Name;

        public bool ElementIsIn = true;

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

        private void Awake()
        {
            if (HudElement == null)
                HudElement = GetComponent<XHud_Module_Element>();

            // 原始姿态数据收集
            OriginalDataCollect();
        }

        /// <summary>
        /// 收集原始姿态数据
        /// </summary>
        public void OriginalDataCollect()
        {
            // 原始姿态数据收集：获取位置
            OriginalPosition = HudElement.RectTransform.anchoredPosition3D;
            // 原始姿态数据收集：获取角度
            OriginalEuler = HudElement.RectTransform.localEulerAngles;
        }

        private void Start()
        {
            if (HideWithStart)
            {
                ElementIsIn = false;
                HudElement.element_AlphaSet(0);
                HudElement.element_Reset(false, false, false);
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素预览器通知", "预览前初始化元素透明度为0！ ", HudMsgState.通知);
            }
            else
            {
                ElementIsIn = true;
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

        private void ControlUpdate()
        {
            if (Input.GetKeyDown(key_Element_In))
            {
                Preview_Element_In();
            }

            if (Input.GetKeyDown(key_Element_Out))
            {
                Preview_Element_Out();
            }

            if (IsEnable)
                HudElement.Element_Animators_GlobalDuration = DurationScaler;
        }

        /// <summary>
        /// 预览Hud元素进入效果
        /// </summary>
        public void Preview_Element_In()
        {
            if (PreviewIsRunning)
                return;
            if (ElementIsIn)
                return;
            PreviewIsRunning = true;

            ElementIsIn = true;

            if (!IsEnable)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素预览器通知", "预览开关已关闭！ ", HudMsgState.错误);
                return;
            }
            if (HudElement.AnimateState == XHudElementAnimateState.Animating)
                return;
            StartCoroutine(Preview_Delay_In());
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素预览器通知", "预览 - 元素进入！ ", HudMsgState.通知);
        }

        /// <summary>
        /// 预览Hud元素退出效果
        /// </summary>
        public void Preview_Element_Out()
        {
            if (PreviewIsRunning)
                return;
            if (!ElementIsIn)
                return;
            PreviewIsRunning = true;

            ElementIsIn = false;

            if (!IsEnable)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素预览器通知", "预览开关已关闭！ ", HudMsgState.错误);
                return;
            }
            if (HudElement.AnimateState == XHudElementAnimateState.Animating)
                return;
            StartCoroutine(Preview_Delay_Out());
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素预览器通知", "预览 - 元素退出！ ", HudMsgState.通知);
        }

        IEnumerator Preview_Delay_In()
        {
            yield return new WaitForSeconds(DelayWithIn);

            if (act_on_element_preview_in_start != null)
                act_on_element_preview_in_start();

            if (RMS_Enabled)
            {
                for (int i = 0; i < HudElement.RMS_LayoutDatas.Count; i++)
                {
                    if (HudElement.RMS_LayoutDatas[i].LayoutName == RMS_Name)
                    {
                        //Debug.Log(OriginalPosition);
                        HudElement.element_PositionSet(OriginalPosition);
                        HudElement.element_AlphaSet(0);
                        XHud_Manager.Instance.hm_ScreenElement_Initialize_By_RMS(HudElement, RMS_Name, true);
                        HudElement.Animators_Rewind();
                        HudElement.Element_In(CreateArgs, () =>
                        {
                            PreviewIsRunning = false;

                            if (act_on_element_preview_in_end != null)
                                act_on_element_preview_in_end();
                        });
                        break;
                    }
                }
            }
            else
            {
                // 进入前先强制设置位置：原始位置
                HudElement.element_PositionSet(OriginalPosition);
                // 进入前强制透明度 0
                HudElement.element_AlphaSet(0);
                // 进入前强制设置角度：原始角度
                HudElement.element_RotationSet(OriginalEuler);
                // 保险操作：倒退动画
                HudElement.Animators_Rewind();
                // 开始进入
                HudElement.Element_In(CreateArgs, () =>
                {
                    PreviewIsRunning = false;

                    if (act_on_element_preview_in_end != null)
                        act_on_element_preview_in_end();
                });
            }
        }

        IEnumerator Preview_Delay_Out()
        {
            yield return new WaitForSeconds(DelayWithOut);

            if (act_on_element_preview_out_start != null)
                act_on_element_preview_out_start();

            HudElement.Element_Out(RecycleArgs, () =>
            {
                PreviewIsRunning = false;

                if (act_on_element_preview_out_end != null)
                    act_on_element_preview_out_end();
            });
        }
    }
}