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
    using SevenStrikeModules.XGUI.Runtime;
    using System.Collections.Generic;
    using UnityEngine;

    public partial class XHud_Module_Option : MonoBehaviour
    {
        [SerializeField]
        /// <summary>
        /// 动画器集合
        /// </summary>
        public List<PrimitiveControllerNode> PrimitiveControllerNodes = new List<PrimitiveControllerNode>();
        [SerializeField]
        /// <summary>
        /// 所有图元动画器中最长的耗时
        /// </summary>
        public float PrimitivesTweenMaxDuration;
        [SerializeField]
        /// <summary>
        /// 所有动画器的动画速度倍乘系数
        /// </summary>
        public float PrimitivesTweenGlobalDuration = 1;
        [SerializeField]
        /// <summary>
        /// 图元列表折叠状态
        /// true表示折叠，false表示展开
        /// 用于控制Inspector中图元列表的显示/隐藏
        /// </summary>
        public bool PrimitivesIsFold;

        #region 获取动画器和动画节点
        /// <summary>
        /// 获取一个PrimitiveController控制器
        /// </summary>
        /// <param name="indicator">目标控制器标识名称</param>
        /// <returns>返回一个匹配标识名称的XHud_Module_Primitive_Controller的控制器</returns>
        public XHud_Module_Primitive_Controller GetPrimitiveController_WithIndicator(string indicator)
        {
            XHud_Module_Primitive_Controller am = null;
            for (int i = 0; i < PrimitiveControllerNodes.Count; i++)
            {
                if (PrimitiveControllerNodes[i].Controller.GetIndicator() == indicator)
                {
                    am = PrimitiveControllerNodes[i].Controller;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 选项控件通知", "未获取到标识名为 " + indicator + " 的子级动画器！ ", XGUIMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 选项控件通知", "已获取子级动画器 " + indicator, XGUIMsgState.通知);
            }
            return am;
        }
        /// <summary>
        /// 获取一个PrimitiveController控制器
        /// </summary>
        /// <param name="name">目标控制器物体名称</param>
        /// <returns>返回一个匹配物体名称名称的XHud_Module_Primitive_Controller的控制器</returns>
        public XHud_Module_Primitive_Controller GetPrimitiveController_WithObjectName(string name)
        {
            XHud_Module_Primitive_Controller am = null;
            for (int i = 0; i < PrimitiveControllerNodes.Count; i++)
            {
                if (PrimitiveControllerNodes[i].Controller.gameObject.name == name)
                {
                    am = PrimitiveControllerNodes[i].Controller;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 选项控件通知", "未获取到名为 " + name + " 的子级动画器！ ", XGUIMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 选项控件通知", "已获取子级动画器 " + name, XGUIMsgState.通知);
            }
            return am;
        }
        /// <summary>
        /// 获取一个PrimitiveController控制器
        /// </summary>
        /// <param name="id">目标控制器的ID</param>
        /// <returns>返回一个匹配ID的XHud_Module_Primitive_Controller的控制器</returns>
        public XHud_Module_Primitive_Controller GetPrimitiveController_WithID(string id)
        {
            XHud_Module_Primitive_Controller am = null;
            for (int i = 0; i < PrimitiveControllerNodes.Count; i++)
            {
                if (PrimitiveControllerNodes[i].Controller.GetID() == id)
                {
                    am = PrimitiveControllerNodes[i].Controller;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 选项控件通知", "未获取到索引号为 " + id + " 的子级动画器！ ", XGUIMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 选项控件通知", "已获取索引号为 " + id + " 子级动画器！", XGUIMsgState.通知);
            }
            return am;
        }
        /// <summary>
        /// 获取一个目标图元控制器上的目标动画节点
        /// </summary>
        /// <param name="primitive_indicator">目标动画器名称</param>
        /// <param name="tween_id">目标动画节点的ID</param>
        /// <returns></returns>
        public TweenNode GetPrimitiveTweenNode_WithIndicator(string primitive_indicator, int tween_id)
        {
            XHud_Module_Primitive_Controller anim = GetPrimitiveController_WithIndicator(primitive_indicator);
            TweenNode node = anim.pt_Tween.TweenNode_GetByID(tween_id);

            if (anim == null)
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 选项控件通知", "未获取到名为 " + primitive_indicator + " 的子级动画器！ ", XGUIMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        XGUI_Utilitys.Console("XHud - 选项控件通知", "已获取子级动画器 " + primitive_indicator, XGUIMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        XGUI_Utilitys.Console("XHud - 选项控件通知", "已获取子级动画器 " + primitive_indicator + "，但并未在其中找到索引号为 " + tween_id + " 的动画效果！", XGUIMsgState.警告);
                }
            }

            return node;
        }
        /// <summary>
        /// 获取一个目标图元控制器上的目标动画节点
        /// </summary>
        /// <param name="primitive_id">目标动画器ID</param>
        /// <param name="tween_id">目标动画节点的ID</param>
        /// <returns></returns>
        public TweenNode GetPrimitiveTweenNode_WithID(string primitive_id, int tween_id)
        {
            XHud_Module_Primitive_Controller anim = GetPrimitiveController_WithID(primitive_id);
            TweenNode node = anim.pt_Tween.TweenNode_GetByID(tween_id);

            if (anim == null)
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 选项控件通知", "未获取到ID为 " + primitive_id + " 的子级动画器！ ", XGUIMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        XGUI_Utilitys.Console("XHud - 选项控件通知", "已获取ID为 " + primitive_id + " 子级动画器", XGUIMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        XGUI_Utilitys.Console("XHud - 选项控件通知", "已获取ID为 " + primitive_id + " 子级动画器，但并未在其中找到ID号为 " + tween_id + " 的动画节点！", XGUIMsgState.警告);
                }
            }

            return node;
        }
        /// <summary>
        /// 获取一个目标图元控制器上的目标动画节点
        /// </summary>
        /// <param name="primitive_indicator">目标动画器名称</param>
        /// <param name="tween_indicator">目标动画节点的名称</param>
        /// <returns></returns>
        public TweenNode GetPrimitiveTweenNode(string primitive_indicator, string tween_indicator)
        {
            XHud_Module_Primitive_Controller anim = GetPrimitiveController_WithIndicator(primitive_indicator);
            TweenNode node = anim.pt_Tween.TweenNode_GetByIndicator(tween_indicator);

            if (anim == null)
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 选项控件通知", "未获取到名为 " + primitive_indicator + " 的子级动画器！ ", XGUIMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        XGUI_Utilitys.Console("XHud - 选项控件通知", "已获取子级动画器 " + primitive_indicator, XGUIMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        XGUI_Utilitys.Console("XHud - 选项控件通知", "已获取子级动画器 " + primitive_indicator + "，但并未在其中找到名称为 " + tween_indicator + " 的动画效果！", XGUIMsgState.警告);
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
        public bool PrimitiveController_IsExist_With_ID(string ID)
        {
            bool isExist = false;
            for (int i = 0; i < PrimitiveControllerNodes.Count; i++)
            {
                if (PrimitiveControllerNodes[i].Controller.GetID() == ID)
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
        public bool PrimitiveController_IsExist_With_Indicator(string Indicator)
        {
            bool isExist = false;
            for (int i = 0; i < PrimitiveControllerNodes.Count; i++)
            {
                if (PrimitiveControllerNodes[i].Controller.GetIndicator() == Indicator)
                {
                    isExist = true;
                }
            }
            return isExist;
        }
        /// <summary>
        /// 播放选项器子级中的所有动画，不包含光标动画
        /// </summary>
        /// <param name="tim">点击选项 | 光标移动开始 | 光标移动结束 | 光标位置改变</param>
        public void PrimitiveTween_Play(string tim = "无")
        {
            if (PrimitiveControllerNodes == null || PrimitiveControllerNodes.Count <= 0)
                return;
            for (int i = 0; i < PrimitiveControllerNodes.Count; i++)
            {
                PrimitiveControllerNode node = PrimitiveControllerNodes[i];
                int v = node.Controller.mod_Rect.GetInstanceID();
                int x = SelectorMark.GetInstanceID();
                if (v == x)
                {
                    continue;
                }
                node.Controller.pt_Tween.Tweens_Play_With_Delay(node.DelayTime, PrimitivesTweenGlobalDuration, true, tim);
            }
            if (DebugState)
                XGUI_Utilitys.Console("XHud - 选项控件通知", "播放图元控制器列表中的所有动画！播放时机为：" + tim.ToString(), XGUIMsgState.通知);
        }
        /// <summary>
        /// 播放按钮子级中的指定ID的动画
        /// </summary>
        /// <param name="id">动画节点的ID</param>
        /// <param name="tim">触发动画的时机</param>
        private void PrimitiveTween_PlayAt(string id, string tim)
        {
            if (PrimitiveControllerNodes == null || PrimitiveControllerNodes.Count <= 0)
                return;

            if (!PrimitiveController_IsExist_With_ID(id))
                return;

            for (int i = 0; i < PrimitiveControllerNodes.Count; i++)
            {
                if (PrimitiveControllerNodes[i].Controller.GetID() != id)
                    continue;

                XHud_Module_Primitive_Controller anim = PrimitiveControllerNodes[i].Controller;
                anim.pt_Tween.Tweens_Play_With_Delay(PrimitiveControllerNodes[i].DelayTime, PrimitivesTweenGlobalDuration, true, tim);
            }

            if (DebugState)
                XGUI_Utilitys.Console("XHud - 选项控件通知", "播放指定ID的图元控制器的动画！播放时机为：" + tim.ToString(), XGUIMsgState.确认);
        }
        /// <summary>
        /// 倒退选项器子级中的所有动画，不包含光标动画
        /// </summary>
        /// <param name="IncludeSelectorMark">忽略光标的动画倒退</param>
        public void PrimitiveTween_Rewind()
        {
            for (int i = 0; i < PrimitiveControllerNodes.Count; i++)
            {
                PrimitiveControllerNode node = PrimitiveControllerNodes[i];
                int v = node.Controller.mod_Rect.GetInstanceID();
                int x = SelectorMark.GetInstanceID();
                if (v == x)
                {
                    continue;
                }
                node.Controller.pt_Tween.Tween_RewindAll();
            }
            if (DebugState)
                XGUI_Utilitys.Console("XHud - 选项控件通知", "倒退复位图元控制器列表中的所有动画！", XGUIMsgState.通知);
        }
        #endregion       
    }
}