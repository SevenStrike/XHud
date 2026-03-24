namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;

    public class xHud_Element_Preview : MonoBehaviour
    {
        public xHud_Module_Element HudElement;

        public bool IsEnable = true;
        public bool DebugState = false;

        public KeyCode key_Element_In = KeyCode.F11;
        public KeyCode key_Element_Out = KeyCode.F12;

        public Motion_Creator CreateArgs;
        public Motion_Recycler RecycleArgs;
        public bool HideWithStart;
        public Vector3 OriginalPosition;
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
                HudElement = GetComponent<xHud_Module_Element>();
            OriginalPosition = HudElement.RectTransform.anchoredPosition3D;
        }

        private void Start()
        {
            if (HideWithStart)
            {
                ElementIsIn = false;
                HudElement.element_AlphaSet(0);
                HudElement.element_Reset(false, false, false);
                if (DebugState)
                    xHud_Utilitys.Func_PrintInfo("元素预览器通知", "预览前初始化元素透明度为0！ ", HudMsgState.通知);
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
                    xHud_Utilitys.Func_PrintInfo("元素预览器通知", "预览开关已关闭！ ", HudMsgState.错误);
                return;
            }
            if (HudElement.AnimateState == HudElementAnimateState.Animating)
                return;
            StartCoroutine(Preview_Delay_In());
            if (DebugState)
                xHud_Utilitys.Func_PrintInfo("元素预览器通知", "预览 - 元素进入！ ", HudMsgState.通知);
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
                    xHud_Utilitys.Func_PrintInfo("元素预览器通知", "预览开关已关闭！ ", HudMsgState.错误);
                return;
            }
            if (HudElement.AnimateState == HudElementAnimateState.Animating)
                return;
            StartCoroutine(Preview_Delay_Out());
            if (DebugState)
                xHud_Utilitys.Func_PrintInfo("元素预览器通知", "预览 - 元素退出！ ", HudMsgState.通知);
        }

        IEnumerator Preview_Delay_In()
        {
            yield return new WaitForSeconds(DelayWithIn);

            if (act_on_element_preview_in_start != null)
                act_on_element_preview_in_start();

            if (RMS_Enabled)
            {
                for (int i = 0; i < HudElement.RMS_InfoList.Count; i++)
                {
                    if (HudElement.RMS_InfoList[i].LayoutName == RMS_Name)
                    {

                        HudElement.element_PositionSet(OriginalPosition);
                        HudElement.element_AlphaSet(0);
                        xHud_Manager.Instance.hm_HudElement_Initialize_ByDesignLayout_For_Screen(HudElement, 0, Vector3.zero, RMS_Name, true);
                        HudElement.Animators_Rewind();
                        HudElement.element_In(CreateArgs, () =>
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
                HudElement.element_PositionSet(OriginalPosition);
                HudElement.element_AlphaSet(0);
                HudElement.Animators_Rewind();
                HudElement.element_In(CreateArgs, () =>
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

            HudElement.element_Out(RecycleArgs, () =>
            {
                PreviewIsRunning = false;

                if (act_on_element_preview_out_end != null)
                    act_on_element_preview_out_end();
            });
        }
    }
}