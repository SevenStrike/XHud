/*
 * ============================================================================
 * ⚠ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠
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
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class XHud_Module_Progress : MonoBehaviour
    {
        public RectTransform OrbitWrapper;
        public RectTransform RectTransform;
        [SerializeField]
        public string Indicator;
        [SerializeField]
        public bool DebugState;

        public HudElementAnimateState AnimateState;

        public float ProgressAnimatorMaxDuration;
        public float Progress_Animators_GlobalDuration = 1;

        #region 内容信息
        public string con_title;
        public string con_subtitle;
        public string ProgressUnit = "%";
        #endregion

        #region 标题文字组件
        /// <summary>
        /// -----------标题
        /// </summary>
        [SerializeField]
        public XHud_Module_Text pro_Text_Title;
        [SerializeField]
        public XHud_Module_TmpText pro_TmpText_Title;

        /// <summary>
        /// -----------副标题
        /// </summary>
        [SerializeField]
        public XHud_Module_Text pro_Text_Subtitle;
        [SerializeField]
        public XHud_Module_TmpText pro_TmpText_Subtitle;

        /// <summary>
        /// -----------数值
        /// </summary>
        [SerializeField]
        public XHud_Module_Text pro_Text_Percent;
        [SerializeField]
        public XHud_Module_TmpText pro_TmpText_Percent;
        #endregion

        #region 事件与委托
        /// <summary>
        /// 动作 - 当 - 进度条开始时
        /// </summary>
        public UnityAction act_on_ValueStart;
        /// <summary>
        /// 动作 - 当 - 进度条变化时
        /// </summary>
        public UnityAction<float> act_on_ValueChanged;
        /// <summary>
        /// 动作 - 当 - 进度条结束时
        /// </summary>
        public UnityAction act_on_ValueEnd;

        /// <summary>
        /// 事件 - 当 - 进度条开始时
        /// </summary>
        public UnityEvent eve_on_ValueStart;
        /// <summary>
        /// 事件 - 当 - 进度条改变时
        /// </summary>
        public UnityEvent<float> eve_on_ValueChanged;
        /// <summary>
        /// 事件 - 当 - 进度条结束时
        /// </summary>
        public UnityEvent eve_on_ValueEnd;
        #endregion

        #region 可视化组件
        public Image pro_Bg;
        public Image pro_Fore;
        public Image pro_Handle;
        public Image pro_Icon;
        #endregion

        #region 进度条值
        [Range(0, 1)]
        public float ProgressValue = 0;
        public float Prev_ProgressValue = 0;
        public float sm_ProgressValue = 0;
        public float ProgressValueDuration = 1f;
        public int ProgressPrecision = 0;
        #endregion

        public List<ElementNode_Animator> ProgressAnimatorNodes;

        public bool LerpMotion = true;
        public bool ProgressIsFinished = false;
        public bool AutoStopPreview = true;
        public bool EventIsFold;
        public bool ProgressAnimatorListIsFold;

        #region 是否显示进度条各种可视化组件
        public bool Display_ProgressRect_Fore = true;
        public bool Display_ProgressRect_Bg = true;
        public bool Display_ProgressRect_Handle = true;
        public bool Display_Icon = true;
        public bool Display_Title = true;
        public bool Display_SubTitle = true;
        public bool Display_Value = true;
        #endregion

        #region 进度条状态
        public bool IsStarted;
        public bool IsEnded;
        #endregion

        private void Awake()
        {
            if (RectTransform == null)
                RectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            pro_ProgressValueReset();
        }

        private void OnEnable()
        {
            pro_Reset();
        }

        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                Animators_Rewind();
                pro_EventsClear();
                pro_ActionsClear();
                pro_Reset(false, true, true, true);
            }
        }

        private void Update()
        {
            pro_Update();
        }

        #region 获取动画器和动画节点
        /// <summary>
        /// 获取一个动画器
        /// </summary>
        /// <param tweenName="indicator">目标标识名称</param>
        /// <returns>返回一个匹配标识名称的HudAnimator动画器</returns>
        public XHud_Module_Animator GetAnimator(string indicator)
        {
            XHud_Module_Animator am = null;
            for (int i = 0; i < ProgressAnimatorNodes.Count; i++)
            {
                if (ProgressAnimatorNodes[i].Animator.GetIndicator() == indicator)
                {
                    am = ProgressAnimatorNodes[i].Animator;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "未获取到标识名为 " + indicator + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "已获取子级动画器 " + indicator, HudMsgState.通知);
            }
            return am;
        }

        /// <summary>
        /// 获取一个动画器
        /// </summary>
        /// <param tweenName="name">目标物体名称</param>
        /// <returns>返回一个匹配物体名称名称的HudAnimator动画器</returns>
        public XHud_Module_Animator GetAnimator_WithObjectName(string name)
        {
            XHud_Module_Animator am = null;
            for (int i = 0; i < ProgressAnimatorNodes.Count; i++)
            {
                if (ProgressAnimatorNodes[i].Animator.gameObject.name == name)
                {
                    am = ProgressAnimatorNodes[i].Animator;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "未获取到名为 " + name + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "已获取子级动画器 " + name, HudMsgState.通知);
            }
            return am;
        }

        /// <summary>
        /// 获取一个动画器
        /// </summary>
        /// <param tweenName="id">目标动画器的ID</param>
        /// <returns>返回一个匹配ID的HudAnimator动画器</returns>
        public XHud_Module_Animator GetAnimator(int id)
        {
            XHud_Module_Animator am = null;
            for (int i = 0; i < ProgressAnimatorNodes.Count; i++)
            {
                if (ProgressAnimatorNodes[i].Animator.GetID() == id)
                {
                    am = ProgressAnimatorNodes[i].Animator;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "未获取到索引号为 " + id + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "已获取索引号为 " + id + " 子级动画器！", HudMsgState.通知);
            }
            return am;
        }

        /// <summary>
        /// 获取一个目标动画器上的目标动画节点
        /// </summary>
        /// <param tweenName="animator_indicator">目标动画器名称</param>
        /// <param tweenName="tween_id">目标动画节点的ID</param>
        /// <returns></returns>
        public TweenNode GetAnimatorTween(string animator_indicator, int tween_id)
        {
            XHud_Module_Animator anim = GetAnimator(animator_indicator);
            TweenNode node = anim.TweenNode_GetByID(tween_id);

            if (anim == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "未获取到名为 " + animator_indicator + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "已获取子级动画器 " + animator_indicator, HudMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "已获取子级动画器 " + animator_indicator + "，但并未在其中找到索引号为 " + tween_id + " 的动画效果！", HudMsgState.警告);
                }
            }

            return node;
        }

        /// <summary>
        /// 获取一个目标动画器上的目标动画节点
        /// </summary>
        /// <param tweenName="animator_id">目标动画器ID</param>
        /// <param tweenName="tween_id">目标动画节点的ID</param>
        /// <returns></returns>
        public TweenNode GetAnimatorTween(int animator_id, int tween_id)
        {
            XHud_Module_Animator anim = GetAnimator(animator_id);
            TweenNode node = anim.TweenNode_GetByID(tween_id);

            if (anim == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "未获取到ID为 " + animator_id + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "已获取ID为 " + animator_id + " 子级动画器", HudMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "已获取ID为 " + animator_id + " 子级动画器，但并未在其中找到ID号为 " + tween_id + " 的动画节点！", HudMsgState.警告);
                }
            }

            return node;
        }

        /// <summary>
        /// 获取一个目标动画器上的目标动画节点
        /// </summary>
        /// <param tweenName="animator_indicator">目标动画器名称</param>
        /// <param tweenName="tween_indicator">目标动画节点的名称</param>
        /// <returns></returns>
        public TweenNode GetAnimatorTween(string animator_indicator, string tween_indicator)
        {
            XHud_Module_Animator anim = GetAnimator(animator_indicator);
            TweenNode node = anim.TweenNode_GetByIndicator(tween_indicator);

            if (anim == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "未获取到名为 " + animator_indicator + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "已获取子级动画器 " + animator_indicator, HudMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "已获取子级动画器 " + animator_indicator + "，但并未在其中找到名称为 " + tween_indicator + " 的动画效果！", HudMsgState.警告);
                }
            }

            return node;
        }
        #endregion

        #region 动画器播放与倒退
        /// <summary>
        /// 验证是否存在指定ID的动画器
        /// </summary>
        /// <returns></returns>
        public bool AnimatorIsExist(int ID)
        {
            bool isExist = false;
            for (int i = 0; i < ProgressAnimatorNodes.Count; i++)
            {
                if (ProgressAnimatorNodes[i].Animator.GetID() == ID)
                {
                    isExist = true;
                }
            }
            return isExist;
        }

        /// <summary>
        /// 验证是否存在指定昵称的动画器
        /// </summary>
        /// <returns></returns>
        public bool AnimatorIsExist(string Indicator)
        {
            bool isExist = false;
            for (int i = 0; i < ProgressAnimatorNodes.Count; i++)
            {
                if (ProgressAnimatorNodes[i].Animator.GetIndicator() == Indicator)
                {
                    isExist = true;
                }
            }
            return isExist;
        }

        /// <summary>
        /// 播放按钮子级中的所有动画
        /// </summary>
        private void Animators_Play(string tim)
        {
            if (ProgressAnimatorNodes == null || ProgressAnimatorNodes.Count <= 0)
                return;

            for (int i = 0; i < ProgressAnimatorNodes.Count; i++)
            {
                XHud_Module_Animator animator = ProgressAnimatorNodes[i].Animator;
                animator.Play(tim, ProgressAnimatorNodes[i].DelayTime, Progress_Animators_GlobalDuration * animator.Animator_GlobalDuration, true, null, null, 0.5f);
            }

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "播放所有动画器动画！", HudMsgState.确认);
        }

        /// <summary>
        /// 播放按钮子级中的指定ID的动画
        /// </summary>
        /// <param tweenName="id">动画节点的ID</param>
        /// <param tweenName="tim">触发动画的时机</param>
        private void Animators_PlayAt(int id, string tim)
        {
            if (ProgressAnimatorNodes == null || ProgressAnimatorNodes.Count <= 0)
                return;

            if (!AnimatorIsExist(id))
                return;

            for (int i = 0; i < ProgressAnimatorNodes.Count; i++)
            {
                if (ProgressAnimatorNodes[i].Animator.GetID() != id)
                    continue;
                XHud_Module_Animator anim = ProgressAnimatorNodes[i].Animator;
                anim.Play(tim, ProgressAnimatorNodes[i].DelayTime, Progress_Animators_GlobalDuration * anim.Animator_GlobalDuration);
            }

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "播放指定ID的动画器的动画！", HudMsgState.确认);
        }

        /// <summary>
        /// 倒退按钮子级中的所有动画
        /// </summary>
        private void Animators_Rewind()
        {
            for (int i = 0; i < ProgressAnimatorNodes.Count; i++)
            {
                XHud_Module_Animator anim = ProgressAnimatorNodes[i].Animator;
                anim.RewindAllTweenNode();
            }
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "复位按钮动画！", HudMsgState.确认);
        }
        #endregion 

        #region 事件动作

        /// <summary>
        /// 移除所有事件
        /// </summary>
        public void pro_EventsClear()
        {
            eve_on_ValueStart.RemoveAllListeners();
            eve_on_ValueEnd.RemoveAllListeners();
            eve_on_ValueChanged.RemoveAllListeners();

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "清空所有事件！", HudMsgState.通知);
        }

        /// <summary>
        /// 移除所有事件
        /// </summary>
        public void pro_ActionsClear()
        {
            act_on_ValueChanged = null;
            act_on_ValueStart = null;
            act_on_ValueEnd = null;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "清空所有委托！", HudMsgState.通知);
        }

        #endregion

        #region 更新
        private void pro_Update()
        {
            if (LerpMotion)
            {
                sm_ProgressValue = Mathf.Lerp(sm_ProgressValue, ProgressValue, Time.deltaTime * ProgressValueDuration);
            }
            else
            {
                sm_ProgressValue = ProgressValue;
            }

            if (!IsStarted)
            {
                if (sm_ProgressValue > 0)
                {
                    IsStarted = true;
                    if (act_on_ValueStart != null)
                        act_on_ValueStart();
                    eve_on_ValueStart.Invoke();
                    Animators_Play("进度开始时");
                }
            }

            if (!IsEnded)
            {
                if (LerpMotion ? sm_ProgressValue >= 0.99f : sm_ProgressValue >= 1)
                {
                    IsEnded = true;
                    if (act_on_ValueEnd != null)
                        act_on_ValueEnd();
                    eve_on_ValueEnd.Invoke();
                    Animators_Play("进度结束时");
                }
            }

            UpdateProgressValueDisplay();

            Prev_ProgressValue = ProgressValue;
        }

        /// <summary>
        /// 更新可视化显示
        /// </summary>
        /// <param tweenName="ForEditorPreview"></param>
        public void UpdateProgressValueDisplay(bool ForEditorPreview = false)
        {
            float prs = 0;
            if (ForEditorPreview)
            {
                prs = ProgressValue;
            }
            else
            {
                prs = sm_ProgressValue;
            }

            if (pro_Text_Percent != null)
            {
                pro_Text_Percent.text = (prs * 100).ToString("F" + ProgressPrecision) + " " + ProgressUnit;
            }

            if (pro_TmpText_Percent != null)
            {
                pro_TmpText_Percent.text = (prs * 100).ToString("F" + ProgressPrecision) + " " + ProgressUnit;
            }

            if (pro_Fore != null)
            {
                if (OrbitWrapper != null)
                {
                    float progress_width = RectTransform.sizeDelta.x;
                    float left_dis = Mathf.Abs(OrbitWrapper.offsetMin.x);
                    float right_dis = Mathf.Abs(OrbitWrapper.offsetMax.x);

                    pro_Fore.rectTransform.offsetMax = new Vector2(-(1 - prs) * (progress_width - (left_dis + right_dis)), pro_Fore.rectTransform.offsetMax.y);
                }
            }
        }

        #endregion

        #region 进度条属性控制
        /// <summary>
        /// 设置进度值
        /// </summary>
        /// <param tweenName="val">进度值</param>
        public void pro_SetProgressValue(float val)
        {
            if (val >= 1)
            {
                ProgressValue = 1;
                return;
            }
            else if (val <= 0)
            {
                ProgressValue = 0;
                return;
            }
            else
                ProgressValue = val;

            if (act_on_ValueChanged != null)
                act_on_ValueChanged(ProgressValue);

            eve_on_ValueChanged.Invoke(ProgressValue);

            if (DebugState && Application.isPlaying)
                XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "设置进度条进度值为：" + val, HudMsgState.通知);
        }

        /// <summary>
        /// 进度值精度设置
        /// </summary>
        public void pro_PrecisionSet(int count)
        {
            ProgressPrecision = count;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "设置进度条精度：" + count, HudMsgState.通知);
        }

        /// <summary>
        /// 单位后缀设置
        /// </summary>
        public void pro_UnitSet(string str)
        {
            ProgressUnit = str;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "设置进度条单位为：" + str, HudMsgState.通知);
        }

        /// <summary>
        /// 重置状态
        /// </summary>
        public void pro_Reset(bool SmoothValueRewind = false, bool ClearEvent = false, bool ClearActions = false, bool ClearText = false)
        {
            ProgressValue = 0;
            if (!SmoothValueRewind)
                sm_ProgressValue = 0;

            if (ClearText)
                pro_TextsClear();
            if (ClearEvent)
                pro_EventsClear();
            if (ClearActions)
                pro_ActionsClear();

            pro_ResetStartEndActions();

            Animators_Rewind();

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "已重置！", HudMsgState.通知);
        }

        /// <summary>
        /// 重置进度条首帧开始与尾帧结束委托状态
        /// </summary>
        private void pro_ResetStartEndActions()
        {
            IsStarted = false;
            IsEnded = false;
        }

        /// <summary>
        /// 重置进度数值状态
        /// </summary>
        public void pro_ProgressValueReset()
        {
            ProgressValue = 0;
            sm_ProgressValue = 0;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "已重置进度条数值！", HudMsgState.通知);
        }

        /// <summary>
        /// 设置进度条的运动方式
        /// </summary>
        /// <param tweenName="smooth"></param>
        public void pro_SetProgreesMotionMode(bool smooth)
        {
            LerpMotion = smooth;
        }
        #endregion

        #region 进度条信息设置
        /// <summary>
        /// 设置进度条标题名称
        /// </summary>
        /// <param tweenName="val"></param>
        public string pro_TitleSet(string val = null)
        {
            con_title = val;

            if (pro_Text_Title != null)
                pro_Text_Title.text = val;

            if (pro_TmpText_Title != null)
                pro_TmpText_Title.text = val;

            if (DebugState && Application.isPlaying)
                XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "设置进度条标题为：" + val, HudMsgState.通知);

            return val;
        }

        /// <summary>
        /// 设置进度条副标题名称
        /// </summary>
        /// <param tweenName="val"></param>
        public string pro_SubTitleSet(string val = null)
        {
            con_subtitle = val;
            if (pro_Text_Subtitle != null)
                pro_Text_Subtitle.text = val;

            if (pro_TmpText_Subtitle != null)
                pro_TmpText_Subtitle.text = val;

            if (DebugState && Application.isPlaying)
                XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "设置进度条副标题为：" + val, HudMsgState.通知);

            return val;
        }

        /// <summary>
        /// 设置进度条标题名称和副标题名称
        /// </summary>
        /// <param tweenName="val"></param>
        public void pro_TitlesSet(string val_title = null, string val_sub = null)
        {
            con_title = val_title;

            if (pro_Text_Title != null)
                pro_Text_Title.text = val_title;

            if (pro_TmpText_Title != null)
                pro_TmpText_Title.text = val_title;

            con_subtitle = val_sub;
            if (pro_Text_Subtitle != null)
                pro_Text_Subtitle.text = val_sub;

            if (pro_TmpText_Subtitle != null)
                pro_TmpText_Subtitle.text = val_sub;
        }

        /// <summary>
        /// 清空标题与副标题名称
        /// </summary>
        public void pro_TextsClear()
        {
            if (pro_Text_Title != null)
                pro_Text_Title.text = null;

            if (pro_TmpText_Title != null)
                pro_TmpText_Title.text = null;

            if (pro_Text_Subtitle != null)
                pro_Text_Subtitle.text = null;

            if (pro_TmpText_Subtitle != null)
                pro_TmpText_Subtitle.text = null;

            con_subtitle = null;
            con_title = null;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "已清空所有内容显示！", HudMsgState.通知);
        }
        #endregion

        #region 进度条可视化设置
        /// <summary>
        /// 设置控制柄的图像
        /// </summary>
        /// <param tweenName="tex"></param>
        public void pro_SetHandle(Texture2D tex)
        {
            pro_Handle.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "设置进度条飞梭图形为：" + tex.name, HudMsgState.通知);
        }

        /// <summary>
        /// 设置前景的图像
        /// </summary>
        /// <param tweenName="tex"></param>
        public void pro_SetFore(Texture2D tex)
        {
            pro_Fore.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "设置进度条前景图形为：" + tex.name, HudMsgState.通知);
        }

        /// <summary>
        /// 设置背景的图像
        /// </summary>
        /// <param tweenName="tex"></param>
        public void pro_SetBg(Texture2D tex)
        {
            pro_Bg.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "设置进度条背景图形为：" + tex.name, HudMsgState.通知);
        }

        /// <summary>
        /// 设置进度条图标
        /// </summary>
        /// <param tweenName="tex"></param>
        public void pro_SetIcon(Texture2D tex)
        {
            pro_Icon.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "设置进度条图标图形为：" + tex.name, HudMsgState.通知);
        }

        /// <summary>
        /// 设置进度条图标
        /// </summary>
        /// <param tweenName="spr"></param>
        public void pro_SetIcon(Sprite spr)
        {
            pro_Icon.sprite = spr;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "设置进度条图标图形为：" + spr.name, HudMsgState.通知);
        }

        /// <summary>
        /// 设置进度条可视化组件的可见性
        /// </summary>
        /// <param tweenName="d_ProgressRect_Fore"></param>
        /// <param tweenName="d_ProgressRect_Bg"></param>
        /// <param tweenName="d_ProgressRect_Handle"></param>
        /// <param tweenName="d_Icon"></param>
        /// <param tweenName="d_Title"></param>
        /// <param tweenName="d_SubTitle"></param>
        /// <param tweenName="d_Value"></param>
        public void pro_SetVisuals(bool d_ProgressRect_Fore, bool d_ProgressRect_Bg, bool d_ProgressRect_Handle, bool d_Icon, bool d_Title, bool d_SubTitle, bool d_Value)
        {
            pro_SetDisplay_ProgressRect_Fore(d_ProgressRect_Fore);
            pro_SetDisplay_ProgressRect_Bg(d_ProgressRect_Bg);
            pro_SetDisplay_ProgressRect_Handle(d_ProgressRect_Handle);
            pro_SetDisplay_Icon(d_Icon);
            pro_SetDisplay_Title(d_Title);
            pro_SetDisplay_SubTitle(d_SubTitle);
            pro_SetDisplay_Value(d_Value);
        }

        /// <summary>
        /// 可视化组件是否可见 - 前景图形
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void pro_SetDisplay_ProgressRect_Fore(bool state)
        {
            Display_ProgressRect_Fore = state;
            if (pro_Fore != null)
            {
                pro_Fore.enabled = state;
            }
        }

        /// <summary>
        /// 可视化组件是否可见 - 背景图形
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void pro_SetDisplay_ProgressRect_Bg(bool state)
        {
            Display_ProgressRect_Bg = state;
            if (pro_Bg != null)
            {
                pro_Bg.enabled = state;
            }
        }

        /// <summary>
        /// 可视化组件是否可见 - 标记
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void pro_SetDisplay_ProgressRect_Handle(bool state)
        {
            Display_ProgressRect_Handle = state;
            if (pro_Handle != null)
            {
                pro_Handle.enabled = state;
            }
        }

        /// <summary>
        /// 可视化组件是否可见 - 图标
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void pro_SetDisplay_Icon(bool state)
        {
            Display_Icon = state;
            if (pro_Icon != null)
            {
                pro_Icon.enabled = state;
            }
        }

        /// <summary>
        /// 可视化组件是否可见 - 标题
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void pro_SetDisplay_Title(bool state)
        {
            Display_Title = state;
            if (pro_Text_Title != null)
            {
                pro_Text_Title.enabled = state;
            }
            if (pro_TmpText_Title != null)
            {
                pro_TmpText_Title.enabled = state;
            }
        }

        /// <summary>
        /// 可视化组件是否可见 - 副标题
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void pro_SetDisplay_SubTitle(bool state)
        {
            Display_SubTitle = state;
            if (pro_Text_Subtitle != null)
            {
                pro_Text_Subtitle.enabled = state;
            }
            if (pro_TmpText_Subtitle != null)
            {
                pro_TmpText_Subtitle.enabled = state;
            }
        }

        /// <summary>
        /// 可视化组件是否可见 - 进度值
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void pro_SetDisplay_Value(bool state)
        {
            Display_Value = state;
            if (pro_Text_Percent != null)
            {
                pro_Text_Percent.enabled = state;
            }
            if (pro_TmpText_Percent != null)
            {
                pro_TmpText_Percent.enabled = state;
            }
        }
        #endregion
    }
}