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
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class XHud_Module_Slider : Slider, IPointerDownHandler, IPointerUpHandler
    {
        public RectTransform Root;
        public RectTransform RectTransform;
        [SerializeField]
        public string Indicator;
        [SerializeField]
        public bool DebugState;

        public float AnimatorsMaxDuration;
        public float Slider_Animators_GlobalDuration = 1f;

        #region 滑动条值
        public string con_title;
        public string con_subtitle;
        public int sli_Precision = 0;
        public string sli_Unit = "%";
        public float Prev_value;
        #endregion

        #region 标题文字组件
        /// <summary>
        /// -----------标题
        /// </summary>
        [SerializeField]
        public XHud_Module_Text sli_Text_Title;
        [SerializeField]
        public XHud_Module_TmpText sli_TmpText_Title;

        /// <summary>
        /// -----------副标题
        /// </summary>
        [SerializeField]
        public XHud_Module_Text sli_Text_Subtitle;
        [SerializeField]
        public XHud_Module_TmpText sli_TmpText_Subtitle;

        /// <summary>
        /// -----------数值
        /// </summary>
        [SerializeField]
        public XHud_Module_Text sli_Text_Percent;
        [SerializeField]
        public XHud_Module_TmpText sli_TmpText_Percent;
        #endregion

        #region 事件动作
        /// <summary>
        /// 动作 - 当 - 滑动条数值变化时
        /// </summary>
        public UnityAction<float> act_on_ValueChanged;
        /// <summary>
        /// 动作 - 当 - 滑动条被按下时
        /// </summary>
        public UnityAction<float> act_on_Press;
        /// <summary>
        /// 动作 - 当 - 松开滑动条时
        /// </summary>
        public UnityAction<float> act_on_Released;

        /// <summary>
        /// 事件 - 当 - 滑动条数值变化时
        /// </summary>
        public UnityEvent<float> eve_on_ValueChanged;
        /// <summary>
        /// 事件 - 当 - 滑动条被按下时
        /// </summary>
        public UnityEvent<float> eve_on_Press;
        /// <summary>
        /// 事件 - 当 - 松开滑动条时
        /// </summary>
        public UnityEvent<float> eve_on_Released;
        #endregion

        #region 可视化组件
        public Image sli_Bg;
        public Image sli_Fore;
        public Image sli_Handle;
        public Image sli_Icon;
        #endregion

        /// <summary>
        /// 动画器集合
        /// </summary>
        public List<ElementNode_Animator> sli_AnimatorNodes = new List<ElementNode_Animator>();

        public HudElementAnimateState AnimateState;

        [SerializeField]
        public bool ToggleOriginalIsFold;
        public bool SliderAnimatorListIsFold;

        public bool AutoStopPreview = true;

        #region 是否显示滑动条各种可视化组件
        public bool Display_Rect_Fore = true;
        public bool Display_Rect_Bg = true;
        public bool Display_Rect_Handle = true;
        public bool Display_Icon = true;
        public bool Display_Title = true;
        public bool Display_SubTitle = true;
        public bool Display_Value = true;
        #endregion

        protected override void Awake()
        {
            base.Awake();
            if (Root == null)
                Root = GetComponentInParent<RectTransform>();

            if (RectTransform == null)
                RectTransform = GetComponent<RectTransform>();
        }

        protected override void Start()
        {
            base.Start();
            sli_SliderValueReset();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            sli_Reset();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (Application.isPlaying)
            {
                Animators_Rewind();
                sli_EventsClear();
                sli_ActionsClear();
                sli_Reset(true, true, true);
            }
        }

        protected override void Update()
        {
            base.Update();
            sli_Update();
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
            for (int i = 0; i < sli_AnimatorNodes.Count; i++)
            {
                if (sli_AnimatorNodes[i].Animator.GetIndicator() == indicator)
                {
                    am = sli_AnimatorNodes[i].Animator;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "未获取到标识名为 " + indicator + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "已获取子级动画器 " + indicator, HudMsgState.通知);
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
            for (int i = 0; i < sli_AnimatorNodes.Count; i++)
            {
                if (sli_AnimatorNodes[i].Animator.gameObject.name == name)
                {
                    am = sli_AnimatorNodes[i].Animator;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "未获取到名为 " + name + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "已获取子级动画器 " + name, HudMsgState.通知);
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
            for (int i = 0; i < sli_AnimatorNodes.Count; i++)
            {
                if (sli_AnimatorNodes[i].Animator.GetID() == id)
                {
                    am = sli_AnimatorNodes[i].Animator;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "未获取到索引号为 " + id + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "已获取索引号为 " + id + " 子级动画器！", HudMsgState.通知);
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
                    XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "未获取到名为 " + animator_indicator + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "已获取子级动画器 " + animator_indicator, HudMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "已获取子级动画器 " + animator_indicator + "，但并未在其中找到索引号为 " + tween_id + " 的动画效果！", HudMsgState.警告);
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
                    XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "未获取到ID为 " + animator_id + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "已获取ID为 " + animator_id + " 子级动画器", HudMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "已获取ID为 " + animator_id + " 子级动画器，但并未在其中找到ID号为 " + tween_id + " 的动画节点！", HudMsgState.警告);
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
                    XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "未获取到名为 " + animator_indicator + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "已获取子级动画器 " + animator_indicator, HudMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "已获取子级动画器 " + animator_indicator + "，但并未在其中找到名称为 " + tween_indicator + " 的动画效果！", HudMsgState.警告);
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
            for (int i = 0; i < sli_AnimatorNodes.Count; i++)
            {
                if (sli_AnimatorNodes[i].Animator.GetID() == ID)
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
            for (int i = 0; i < sli_AnimatorNodes.Count; i++)
            {
                if (sli_AnimatorNodes[i].Animator.GetIndicator() == Indicator)
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
            if (sli_AnimatorNodes == null || sli_AnimatorNodes.Count <= 0)
                return;

            for (int i = 0; i < sli_AnimatorNodes.Count; i++)
            {
                XHud_Module_Animator animator = sli_AnimatorNodes[i].Animator;
                animator.Play(tim, sli_AnimatorNodes[i].DelayTime, Slider_Animators_GlobalDuration * animator.Animator_GlobalDuration, true, null, null, 0.5f);
            }

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "播放所有动画器动画！", HudMsgState.确认);
        }

        /// <summary>
        /// 播放按钮子级中的指定ID的动画
        /// </summary>
        /// <param tweenName="id">动画节点的ID</param>
        /// <param tweenName="tim">触发动画的时机</param>
        private void Animators_PlayAt(int id, string tim)
        {
            if (sli_AnimatorNodes == null || sli_AnimatorNodes.Count <= 0)
                return;

            if (!AnimatorIsExist(id))
                return;

            for (int i = 0; i < sli_AnimatorNodes.Count; i++)
            {
                if (sli_AnimatorNodes[i].Animator.GetID() != id)
                    continue;
                XHud_Module_Animator anim = sli_AnimatorNodes[i].Animator;
                anim.Play(tim, sli_AnimatorNodes[i].DelayTime, Slider_Animators_GlobalDuration * anim.Animator_GlobalDuration);
            }

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "播放指定ID的动画器的动画！", HudMsgState.确认);
        }

        /// <summary>
        /// 倒退按钮子级中的所有动画
        /// </summary>
        private void Animators_Rewind()
        {
            for (int i = 0; i < sli_AnimatorNodes.Count; i++)
            {
                XHud_Module_Animator anim = sli_AnimatorNodes[i].Animator;
                anim.RewindAllTweenNode();
            }
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "复位按钮动画！", HudMsgState.确认);
        }
        #endregion       

        #region 事件动作

        /// <summary>
        /// 移除所有事件
        /// </summary>
        public void sli_EventsClear()
        {
            eve_on_ValueChanged.RemoveAllListeners();
            eve_on_Press.RemoveAllListeners();
            eve_on_Released.RemoveAllListeners();

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "清空所有事件！", HudMsgState.通知);
        }

        /// <summary>
        /// 移除所有事件
        /// </summary>
        public void sli_ActionsClear()
        {
            act_on_ValueChanged = null;
            act_on_Press = null;
            act_on_Released = null;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "清空所有委托！", HudMsgState.通知);
        }

        #endregion

        #region 辅助
        /// <summary>
        /// 重置状态
        /// </summary>
        public void sli_Reset(bool ClearEvent = false, bool ClearActions = false, bool ClearText = false)
        {
            value = 0;

            if (ClearText)
                sli_TextsClear();
            if (ClearEvent)
                sli_EventsClear();
            if (ClearActions)
                sli_ActionsClear();

            Animators_Rewind();

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "已重置！", HudMsgState.通知);
        }

        /// <summary>
        /// 重置进度数值状态
        /// </summary>
        public void sli_SliderValueReset()
        {
            value = 0;
        }

        /// <summary>
        /// 设置控制柄的启用或禁用
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void sli_SetInteractable(bool state)
        {
            interactable = state;

            if (state)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "启用按钮交互", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "禁用按钮交互", HudMsgState.通知);
            }
        }
        #endregion

        #region 更新
        private void sli_Update()
        {
            if (Application.isPlaying)
            {
                UpdateSliderValueDisplay();

                Prev_value = value;
            }
        }

        /// <summary>
        /// 更新可视化显示
        /// </summary>
        public void UpdateSliderValueDisplay()
        {
            float prs = 0;

            prs = value;

            if (sli_Text_Percent != null)
            {
                sli_Text_Percent.text = (prs).ToString("F" + sli_Precision) + " " + sli_Unit;
            }

            if (sli_TmpText_Percent != null)
            {
                sli_TmpText_Percent.text = (prs).ToString("F" + sli_Precision) + " " + sli_Unit;
            }
        }

        #endregion

        #region 滑动条属性控制
        /// <summary>
        /// 设置滑动条的位置
        /// </summary>
        /// <param tweenName="val"></param>
        public void sli_SetSliderValue(float val)
        {
            value = val;

            if (act_on_ValueChanged != null)
                act_on_ValueChanged(value);

            eve_on_ValueChanged.Invoke(value);

            if (DebugState && Application.isPlaying)
                XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "设置滑动条进度值为：" + val, HudMsgState.通知);
        }

        /// <summary>
        /// 设置滑动条的位置
        /// </summary>
        /// <param tweenName="val"></param>
        public void sli_SetSliderValue(float val, float min, float max)
        {
            value = val;
            minValue = min;
            maxValue = max;

            if (act_on_ValueChanged != null)
                act_on_ValueChanged(value);

            eve_on_ValueChanged.Invoke(value);

            if (DebugState && Application.isPlaying)
                XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "设置滑动条进度值为：" + val, HudMsgState.通知);
        }

        /// <summary>
        /// 设置滑动条的范围
        /// </summary>
        /// <param tweenName="val"></param>
        public void sli_SetSliderValue(float min, float max)
        {
            minValue = min;
            maxValue = max;

            if (DebugState && Application.isPlaying)
                XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", $"设置滑动条范围值为：{min} - {max}", HudMsgState.通知);
        }

        /// <summary>
        /// 数值精度设置
        /// </summary>
        public void sli_PrecisionSet(int count)
        {
            sli_Precision = count;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "设置滑动条精度：" + count, HudMsgState.通知);
        }

        /// <summary>
        /// 单位后缀设置
        /// </summary>
        public void sli_UnitSet(string str)
        {
            sli_Unit = str;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "设置滑动条单位为：" + str, HudMsgState.通知);
        }
        #endregion

        #region 滑动条信息设置
        /// <summary>
        /// 设置滑动条标题名称
        /// </summary>
        /// <param tweenName="val"></param>
        public string sli_TitleSet(string val = null)
        {
            con_title = val;

            if (sli_Text_Title != null)
                sli_Text_Title.text = val;

            if (sli_TmpText_Title != null)
                sli_TmpText_Title.text = val;

            if (DebugState && Application.isPlaying)
                XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "设置标题文字为：" + val, HudMsgState.通知);

            return val;
        }

        /// <summary>
        /// 设置滑动条副标题名称
        /// </summary>
        /// <param tweenName="val"></param>
        public string sli_SubTitleSet(string val = null)
        {
            con_subtitle = val;

            if (sli_Text_Subtitle != null)
                sli_Text_Subtitle.text = val;

            if (sli_TmpText_Subtitle != null)
                sli_TmpText_Subtitle.text = val;

            if (DebugState && Application.isPlaying)
                XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "设置副标题文字为：" + val, HudMsgState.通知);

            return val;
        }

        /// <summary>
        /// 设置进度条标题名称和副标题名称
        /// </summary>
        /// <param tweenName="val"></param>
        public void sli_TitlesSet(string val_title = null, string val_sub = null)
        {
            con_title = val_title;

            if (sli_Text_Title != null)
                sli_Text_Title.text = val_title;

            if (sli_TmpText_Title != null)
                sli_TmpText_Title.text = val_title;

            con_subtitle = val_sub;
            if (sli_Text_Subtitle != null)
                sli_Text_Subtitle.text = val_sub;

            if (sli_TmpText_Subtitle != null)
                sli_TmpText_Subtitle.text = val_sub;
        }

        /// <summary>
        /// 清空标题与副标题名称
        /// </summary>
        public void sli_TextsClear()
        {
            if (sli_Text_Title != null)
                sli_Text_Title.text = null;

            if (sli_TmpText_Title != null)
                sli_TmpText_Title.text = null;

            con_title = null;

            if (sli_Text_Subtitle != null)
                sli_Text_Subtitle.text = null;

            if (sli_TmpText_Subtitle != null)
                sli_TmpText_Subtitle.text = null;

            con_subtitle = null;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "清空所有文字内容！", HudMsgState.通知);
        }
        #endregion

        #region 进度条可视化设置
        /// <summary>
        /// 设置控制柄的图像
        /// </summary>
        /// <param tweenName="tex"></param>
        public void sli_SetHandle(Texture2D tex)
        {
            sli_Handle.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "设置滑动条控制柄图形为：" + tex.name, HudMsgState.通知);
        }

        /// <summary>
        /// 设置前景的图像
        /// </summary>
        /// <param tweenName="tex"></param>
        public void sli_SetFore(Texture2D tex)
        {
            sli_Fore.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "设置滑动条前景图形为：" + tex.name, HudMsgState.通知);
        }

        /// <summary>
        /// 设置背景的图像
        /// </summary>
        /// <param tweenName="tex"></param>
        public void sli_SetBg(Texture2D tex)
        {
            sli_Bg.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "设置滑动条背景图形为：" + tex.name, HudMsgState.通知);
        }

        /// <summary>
        /// 设置滑动条图标
        /// </summary>
        /// <param tweenName="tex"></param>
        public void sli_SetIcon(Texture2D tex)
        {
            sli_Icon.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "设置滑动条图标图形为：" + tex.name, HudMsgState.通知);
        }

        /// <summary>
        /// 设置滑动条图标
        /// </summary>
        /// <param tweenName="spr"></param>
        public void sli_SetIcon(Sprite spr)
        {
            sli_Icon.sprite = spr;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 进度条控件通知", "设置滑动条图标图形为：" + spr.name, HudMsgState.通知);
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
        public void sli_SetVisuals(bool d_ProgressRect_Fore, bool d_ProgressRect_Bg, bool d_ProgressRect_Handle, bool d_Icon, bool d_Title, bool d_SubTitle, bool d_Value)
        {
            sli_SetDisplay_ProgressRect_Fore(d_ProgressRect_Fore);
            sli_SetDisplay_ProgressRect_Bg(d_ProgressRect_Bg);
            sli_SetDisplay_ProgressRect_Handle(d_ProgressRect_Handle);
            sli_SetDisplay_Icon(d_Icon);
            sli_SetDisplay_Title(d_Title);
            sli_SetDisplay_SubTitle(d_SubTitle);
            sli_SetDisplay_Value(d_Value);
        }

        /// <summary>
        /// 可视化组件是否可见 - 前景图形
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void sli_SetDisplay_ProgressRect_Fore(bool state)
        {
            Display_Rect_Fore = state;
            if (sli_Fore != null)
            {
                sli_Fore.enabled = state;
            }
        }

        /// <summary>
        /// 可视化组件是否可见 - 背景图形
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void sli_SetDisplay_ProgressRect_Bg(bool state)
        {
            Display_Rect_Bg = state;
            if (sli_Bg != null)
            {
                sli_Bg.enabled = state;
            }
        }

        /// <summary>
        /// 可视化组件是否可见 - 标记
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void sli_SetDisplay_ProgressRect_Handle(bool state)
        {
            Display_Rect_Handle = state;
            if (sli_Handle != null)
            {
                sli_Handle.enabled = state;
            }
        }

        /// <summary>
        /// 可视化组件是否可见 - 图标
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void sli_SetDisplay_Icon(bool state)
        {
            Display_Icon = state;
            if (sli_Icon != null)
            {
                sli_Icon.enabled = state;
            }
        }

        /// <summary>
        /// 可视化组件是否可见 - 标题
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void sli_SetDisplay_Title(bool state)
        {
            Display_Title = state;
            if (sli_Text_Title != null)
            {
                sli_Text_Title.enabled = state;
            }
            if (sli_TmpText_Title != null)
            {
                sli_TmpText_Title.enabled = state;
            }
        }

        /// <summary>
        /// 可视化组件是否可见 - 副标题
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void sli_SetDisplay_SubTitle(bool state)
        {
            Display_SubTitle = state;
            if (sli_Text_Subtitle != null)
            {
                sli_Text_Subtitle.enabled = state;
            }
            if (sli_TmpText_Subtitle != null)
            {
                sli_TmpText_Subtitle.enabled = state;
            }
        }

        /// <summary>
        /// 可视化组件是否可见 - 进度值
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void sli_SetDisplay_Value(bool state)
        {
            Display_Value = state;
            if (sli_Text_Percent != null)
            {
                sli_Text_Percent.enabled = state;
            }
            if (sli_TmpText_Percent != null)
            {
                sli_TmpText_Percent.enabled = state;
            }
        }
        #endregion

        #region 关联原生事件

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (act_on_Press != null)
                act_on_Press(value);
            eve_on_Press.Invoke(value);
            Animators_Play("按下滑动条");

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "按住！", HudMsgState.通知);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (act_on_Released != null)
                act_on_Released(value);
            eve_on_Released.Invoke(value);
            Animators_Play("松开滑动条");

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 滑动条控件通知", "松开！", HudMsgState.通知);
        }

        #endregion

    }
}