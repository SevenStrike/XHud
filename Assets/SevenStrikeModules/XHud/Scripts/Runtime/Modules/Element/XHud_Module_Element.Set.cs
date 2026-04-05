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
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections.Generic;
    using UnityEngine;

    public partial class XHud_Module_Element : MonoBehaviour
    {
        /// <summary>
        /// XHud元素 - 交互总开关
        /// </summary>
        /// <param name="state">交互开关状态</param>
        public virtual void element_SetInteractable(bool state)
        {
            CanvasGroup.interactable = state;
            element_Button_InteractableSetAll(state);
            element_Option_InteractableSetAll(state);
            element_Slider_InteractableSetAll(state);
            element_Progress_EnableSetAll(state);
            element_Toggle_InteractableSetAll(state);
            if (state)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "将所有下级控件的交互开启！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "将所有下级控件的交互禁用！", HudMsgState.通知);
            }
        }
        /// <summary>
        /// XHud元素 - 生成后自动播放动画的开关
        /// </summary>
        /// <param name="state">开关状态</param>
        public virtual void element_SetAutoAnimator(bool state)
        {
            AutoPlayAnimators = state;
        }
        /// <summary>
        /// XHud元素 - 重置状态
        /// </summary>
        /// <param name="Hidden">重置后是否隐藏物体</param>
        /// <param name="ClearEvents">清空事件</param>
        /// <param name="ClearActions">清空动作</param>
        public virtual void element_Reset(bool Hidden = true, bool ClearEvents = true, bool ClearActions = true)
        {
            ID = 0;
            element_AlphaSet(0);

            CurrentPivot = Vector2.one * 0.5f;

            if (CanvasGroup != null)
                CanvasGroup.interactable = true;

            if (ClearEvents)
            {
                act_on_element_in_start = null;
                act_on_element_in_progress = null;
                act_on_element_in_end = null;
                act_on_element_out_start = null;
                act_on_element_out_progress = null;
                act_on_element_out_end = null;
            }
            if (ClearActions)
            {
                eve_on_element_in_start.RemoveAllListeners();
                eve_on_element_in_end.RemoveAllListeners();
                eve_on_element_out_start.RemoveAllListeners();
                eve_on_element_out_end.RemoveAllListeners();
            }

            if (Tween_Alpha != null)
            {
                Tween_Alpha.Kill();
                Tween_Alpha = null;
            }

            if (Tween_Move != null)
            {
                Tween_Move.Kill();
                Tween_Move = null;
            }

            if (Tween_Rotation != null)
            {
                Tween_Rotation.Kill();
                Tween_Rotation = null;
            }

            Animators_Rewind();
            CreateState = XHudElementCreateState.Recycled;
            gameObject.SetActive(!Hidden);

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "已重置！", HudMsgState.警告);
        }
        /// <summary>
        /// XHud元素 - 设置锚点
        /// </summary>
        /// <param name="pivot">锚点</param>
        public virtual void element_PivotSet(Vector2 pivot)
        {
            CurrentPivot = pivot;
            RectTransform.pivot = pivot;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "元素的锚点设置为：" + pivot, HudMsgState.通知);
        }
        /// <summary>
        /// XHud元素 - 设置锚点
        /// </summary>
        /// <param name="type">锚点类型</param>
        public virtual void element_PivotSet(XHudAnchor type)
        {
            switch (type)
            {
                case XHudAnchor.上:
                    CurrentPivot = new Vector2(0.5f, 1f);
                    break;
                case XHudAnchor.下:
                    CurrentPivot = new Vector2(0.5f, 0f);
                    break;
                case XHudAnchor.左:
                    CurrentPivot = new Vector2(0f, 0.5f);
                    break;
                case XHudAnchor.右:
                    CurrentPivot = new Vector2(1f, 0.5f);
                    break;
                case XHudAnchor.中心:
                    CurrentPivot = new Vector2(0.5f, 0.5f);
                    break;
                case XHudAnchor.左上:
                    CurrentPivot = new Vector2(0f, 1f);
                    break;
                case XHudAnchor.左下:
                    CurrentPivot = new Vector2(0f, 0f);
                    break;
                case XHudAnchor.右上:
                    CurrentPivot = new Vector2(1f, 1f);
                    break;
                case XHudAnchor.右下:
                    CurrentPivot = new Vector2(1f, 0f);
                    break;
                case XHudAnchor.底层:
                    CurrentPivot = new Vector2(0.5f, 0.5f);
                    break;
                case XHudAnchor.顶层:
                    CurrentPivot = new Vector2(0.5f, 0.5f);
                    break;
            }
            RectTransform.pivot = CurrentPivot;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "元素的锚点设置为：" + type.ToString(), HudMsgState.通知);
        }
        /// <summary>
        /// XHud元素 - 设置锚点区域范围
        /// </summary>
        /// <param name="AnchorMin"></param>
        /// <param name="AnchorMax"></param>
        public virtual void element_AnchorRangeSet(Vector2 AnchorMin, Vector2 AnchorMax)
        {
            RectTransform.anchorMin = AnchorMin;
            RectTransform.anchorMax = AnchorMax;
        }
        /// <summary>
        /// XHud元素 - 位置归零
        /// </summary>
        public virtual void element_PositionResetZero()
        {
            RectTransform.anchoredPosition3D = Vector3.zero;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "元素的3D锚点位置归零！", HudMsgState.通知);
        }
        /// <summary>
        /// XHud元素 - 旋转归零
        /// </summary>
        public virtual void element_RotationResetZero()
        {
            RectTransform.localEulerAngles = Vector3.zero;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "元素的旋转归零！", HudMsgState.通知);
        }
        /// <summary>
        /// XHud元素 - 位置设置
        /// </summary>
        /// <param name="pos">锚点位置_AnchoredPosition</param>
        public virtual void element_PositionSet(Vector3 pos)
        {
            RectTransform.anchoredPosition3D = pos;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "元素的锚点位置设置为：" + pos, HudMsgState.通知);
        }
        /// <summary>
        /// XHud元素 - 尺寸设置
        /// </summary>
        /// <param name="size">尺寸_Size</param>
        public virtual void element_SizeSet(Vector2 size)
        {
            RectTransform.sizeDelta = size;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "元素的尺寸设置为：" + size, HudMsgState.通知);
        }
        /// <summary>
        /// XHud元素 - 位置设置 - 世界
        /// </summary>
        /// <param name="pos">世界位置</param>
        public virtual void element_WorldPositionSet(Vector3 pos)
        {
            RectTransform.position = pos;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "元素的世界位置设置为：" + pos, HudMsgState.通知);
        }
        /// <summary>
        /// XHud元素 - 旋转设置
        /// </summary>
        /// <param name="rot">本地旋转角度</param>
        public virtual void element_RotationSet(Vector3 rot)
        {
            RectTransform.localEulerAngles = rot;
        }
        /// <summary>
        /// XHud元素 - 旋转设置 - 世界
        /// </summary>
        /// <param name="rot">世界旋转角度</param>
        public virtual void element_WorldRotationSet(Quaternion rot)
        {
            RectTransform.rotation = rot;
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "元素的世界旋转设置为：" + rot, HudMsgState.通知);
        }
        /// <summary>
        /// XHud元素 - 位置偏移设置
        /// </summary>
        /// <param name="offset">空间偏移</param>
        public virtual void element_PositionOffset(Vector3 offset)
        {
            RectTransform.anchoredPosition3D += offset;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "元素的3D锚点位置偏移设置为：" + offset, HudMsgState.通知);
        }
        /// <summary>
        /// XHud元素 - 缩放设置
        /// </summary>
        /// <param name="sca">缩放大小</param>
        public virtual void element_ScaleSet(Vector3 sca)
        {
            RectTransform.localScale = sca;
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "元素的缩放设置为：" + sca, HudMsgState.通知);
        }
        /// <summary>
        /// XHud元素 - 生成源类型设置
        /// </summary>
        /// <param name="type">生成源类型</param>
        public virtual void element_CreatedSourceTypeSet(XHudElementCreatedSourceType type)
        {
            CreatedSourceType = type;
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "元素的生成源类型设置为：" + type, HudMsgState.通知);
        }
        /// <summary>
        /// XHud元素 - 检查自身是否已从对象池生成或者为启用/禁用
        /// </summary>
        /// <returns>返回True：启用并且当前已被对象池生成到场景，返回False：禁用并且在对象池中</returns>
        public virtual bool element_ActiveState()
        {
            return gameObject.activeSelf;
        }
        /// <summary>
        /// XHud元素 - 创建ID编号
        /// </summary>
        /// <param name="ElementNodes"></param>
        /// <returns></returns>
        public virtual int element_CreateID(XHudElementNode[] ElementNodes)
        {
            List<int> ids = new List<int>();
            for (int i = 0; i < ElementNodes.Length; i++)
            {
                ids.Add(ElementNodes[i].ID);
            }

            int ran_id = Random.Range(1111, 9999);
            while (true)
            {
                if (ids.Contains(ran_id))
                {
                    ran_id = Random.Range(111111, 999999);
                }
                else
                {
                    this.ID = ran_id;
                    break;
                }
            }

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "生成的随机ID为：" + ran_id, HudMsgState.通知);
            return ran_id;
        }
    }
}