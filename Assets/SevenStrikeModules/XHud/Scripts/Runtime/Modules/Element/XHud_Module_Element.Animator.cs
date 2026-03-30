namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 动画器的结构类
    /// </summary>
    [System.Serializable]
    public class ElementNode_Animator
    {
        [SerializeField]
        /// <summary>
        /// 动画器
        /// </summary>
        public XHud_Module_Animator Animator;
        [SerializeField]
        /// <summary>
        /// 动画器的动画总计时间
        /// </summary>
        public float TotalTime;
        [SerializeField]
        /// <summary>
        /// 动画器的动画延迟时间
        /// </summary>
        public float DelayTime;
    }

    public partial class XHud_Module_Element : MonoBehaviour
    {
        [SerializeField]
        /// <summary>
        /// 动画器容器
        /// </summary>
        public List<ElementNode_Animator> AnimatorNodes;
        [SerializeField]
        /// <summary>
        /// 所有动画器中最长的耗时
        /// </summary>
        public float AnimatorsMaxDuration;
        [SerializeField]
        /// <summary>
        /// 元素下所有动画器的动画速度倍乘系数
        /// </summary>
        public float Element_Animators_GlobalDuration = 1;
        [SerializeField]
        /// <summary>
        /// 动画器自动播放开关
        /// true表示元素生成时自动播放所有动画器
        /// false表示需要手动调用播放
        /// 默认值为true
        /// </summary>
        public bool AutoPlayAnimators = true;
        [SerializeField]
        /// <summary>
        /// 动画器列表折叠状态
        /// true表示折叠，false表示展开
        /// 用于控制Inspector中动画器列表的显示/隐藏
        /// </summary>
        public bool AnimatorsIsFold;

        #region 获取动画器和动画节点
        /// <summary>
        /// 获取一个动画器
        /// </summary>
        /// <param name="indicator">目标名称</param>
        /// <returns>返回一个匹配名称的HudAnimator动画器</returns>
        public XHud_Module_Animator GetAnimator(string indicator)
        {
            XHud_Module_Animator am = null;
            for (int i = 0; i < AnimatorNodes.Count; i++)
            {
                if (AnimatorNodes[i].Animator.GetIndicator() == indicator)
                {
                    am = AnimatorNodes[i].Animator;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "未获取到名为 " + indicator + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "已获取子级动画器 " + indicator, HudMsgState.通知);
            }
            return am;
        }

        /// <summary>
        /// 获取一个动画器
        /// </summary>
        /// <param name="name">目标物体名称</param>
        /// <returns>返回一个匹配物体名称名称的HudAnimator动画器</returns>
        public XHud_Module_Animator GetAnimator_WithObjectName(string name)
        {
            XHud_Module_Animator am = null;
            for (int i = 0; i < AnimatorNodes.Count; i++)
            {
                if (AnimatorNodes[i].Animator.gameObject.name == name)
                {
                    am = AnimatorNodes[i].Animator;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "未获取到名为 " + name + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "已获取子级动画器 " + name, HudMsgState.通知);
            }
            return am;
        }

        /// <summary>
        /// Hud元素 - 获取一个动画器
        /// </summary>
        /// <param name="id">目标ID</param>
        /// <returns>返回一个匹配ID的HudAnimator动画器</returns>
        public XHud_Module_Animator GetAnimator(int id)
        {
            XHud_Module_Animator am = null;
            for (int i = 0; i < AnimatorNodes.Count; i++)
            {
                if (AnimatorNodes[i].Animator.GetID() == id)
                {
                    am = AnimatorNodes[i].Animator;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "未获取到索引号为 " + id + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "已获取索引号为 " + id + " 子级动画器！", HudMsgState.通知);
            }
            return am;
        }

        /// <summary>
        /// 获取一个目标动画器上的目标动画节点
        /// </summary>
        /// <param name="animator_indicator">目标动画器名称</param>
        /// <param name="tween_id">目标动画节点的ID</param>
        /// <returns></returns>
        public TweenNode GetAnimatorTween(string animator_indicator, int tween_id)
        {
            XHud_Module_Animator anim = GetAnimator(animator_indicator);
            TweenNode node = anim.TweenNode_GetByID(tween_id);

            if (anim == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "未获取到名为 " + animator_indicator + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("元素控件通知", "已获取子级动画器 " + animator_indicator, HudMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("元素控件通知", "已获取子级动画器 " + animator_indicator + "，但并未在其中找到索引号为 " + tween_id + " 的动画效果！", HudMsgState.警告);
                }
            }

            return node;
        }

        /// <summary>
        /// 获取一个目标动画器上的目标动画节点
        /// </summary>
        /// <param name="animator_id">目标动画器ID</param>
        /// <param name="tween_id">目标动画节点的ID</param>
        /// <returns></returns>
        public TweenNode GetAnimatorTween(int animator_id, int tween_id)
        {
            XHud_Module_Animator anim = GetAnimator(animator_id);
            TweenNode node = anim.TweenNode_GetByID(tween_id);

            if (anim == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "未获取到ID为 " + animator_id + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("元素控件通知", "已获取ID为 " + animator_id + " 子级动画器", HudMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("元素控件通知", "已获取ID为 " + animator_id + " 子级动画器，但并未在其中找到ID号为 " + tween_id + " 的动画节点！", HudMsgState.警告);
                }
            }

            return node;
        }

        /// <summary>
        /// 获取一个目标动画器上的目标动画节点
        /// </summary>
        /// <param name="animator_indicator">目标动画器名称</param>
        /// <param name="tween_indicator">目标动画节点的名称</param>
        /// <returns></returns>
        public TweenNode GetAnimatorTween(string animator_indicator, string tween_indicator)
        {
            XHud_Module_Animator anim = GetAnimator(animator_indicator);
            TweenNode node = anim.TweenNode_GetByIndicator(tween_indicator);

            if (anim == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "未获取到名为 " + animator_indicator + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("元素控件通知", "已获取子级动画器 " + animator_indicator, HudMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("元素控件通知", "已获取子级动画器 " + animator_indicator + "，但并未在其中找到名称为 " + tween_indicator + " 的动画效果！", HudMsgState.警告);
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
            for (int i = 0; i < AnimatorNodes.Count; i++)
            {
                if (AnimatorNodes[i].Animator.GetID() == ID)
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
            for (int i = 0; i < AnimatorNodes.Count; i++)
            {
                if (AnimatorNodes[i].Animator.GetIndicator() == Indicator)
                {
                    isExist = true;
                }
            }
            return isExist;
        }

        /// <summary>
        /// Hud元素 - 播放所有子元素动画器的动画
        /// </summary>
        /// <returns></returns>
        public void Animators_Play(string tim)
        {
            if (AnimatorNodes == null || AnimatorNodes.Count <= 0)
                return;
            for (int i = 0; i < AnimatorNodes.Count; i++)
            {
                XHud_Module_Animator anim = AnimatorNodes[i].Animator;
                anim.Play(tim, AnimatorNodes[i].DelayTime, Element_Animators_GlobalDuration * anim.Animator_GlobalDuration);
            }
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "播放动画器列表中的所有动画！播放时机为：" + tim.ToString(), HudMsgState.通知);
        }

        /// <summary>
        /// 播放按钮子级中的指定ID的动画
        /// </summary>
        /// <param name="id">动画节点的ID</param>
        /// <param name="tim">触发动画的时机</param>
        private void Animators_PlayAt(int id, string tim)
        {
            if (AnimatorNodes == null || AnimatorNodes.Count <= 0)
                return;

            if (!AnimatorIsExist(id))
                return;

            for (int i = 0; i < AnimatorNodes.Count; i++)
            {
                if (AnimatorNodes[i].Animator.GetID() != id)
                    continue;
                XHud_Module_Animator anim = AnimatorNodes[i].Animator;
                anim.Play(tim, AnimatorNodes[i].DelayTime, Element_Animators_GlobalDuration * anim.Animator_GlobalDuration);
            }

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "播放指定ID的动画器的动画！", HudMsgState.确认);
        }

        /// <summary>
        /// Hud元素 - 倒退所有子元素动画器的动画
        /// </summary>
        /// <returns></returns>
        public void Animators_Rewind()
        {
            for (int i = 0; i < AnimatorNodes.Count; i++)
            {
                XHud_Module_Animator anim = AnimatorNodes[i].Animator;
                anim.RewindAllTweenNode();
            }

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "倒退复位动画器列表中的所有动画！", HudMsgState.通知);
        }
        #endregion              

    }
}