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
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class XHud_Element_TriggerAction : EventTrigger
    {
        public Image TriggerImage;
        public XHud_Module_Element HudElement;
        public bool AutoClearActionsAndEvents;

        /// <summary>
        /// 动作 - 当 - 鼠标点击时
        /// </summary>
        public UnityAction<XHud_Module_Element, PointerEventData> act_on_Clicked;
        /// <summary>
        /// 动作 - 当 - 鼠标按下时
        /// </summary>
        public UnityAction<XHud_Module_Element, PointerEventData> act_on_Press;
        /// <summary>
        /// 动作 - 当 - 鼠标进入时
        /// </summary>
        public UnityAction<XHud_Module_Element, PointerEventData> act_on_Enter;
        /// <summary>
        /// 动作 - 当 - 鼠标退出时
        /// </summary>
        public UnityAction<XHud_Module_Element, PointerEventData> act_on_Exit;
        /// <summary>
        /// 动作 - 当 - 鼠标抬起时
        /// </summary>
        public UnityAction<XHud_Module_Element, PointerEventData> act_on_Release;
        /// <summary>
        /// 动作 - 当 - 鼠标选中时
        /// </summary>
        public UnityAction<XHud_Module_Element, BaseEventData> act_on_Select;
        /// <summary>
        /// 动作 - 当 - 鼠标取消选中时
        /// </summary>
        public UnityAction<XHud_Module_Element, BaseEventData> act_on_Unselect;
        /// <summary>
        /// 动作 - 当 - 鼠标拖拽时
        /// </summary>
        public UnityAction<XHud_Module_Element, BaseEventData> act_on_Drag;
        /// <summary>
        /// 动作 - 当 - 鼠标放下时
        /// </summary>
        public UnityAction<XHud_Module_Element, BaseEventData> act_on_Drop;

        /// <summary>
        /// 事件 - 当 - 鼠标点击时
        /// </summary>
        public UnityEvent eve_on_pointer_Clicked;
        /// <summary>
        /// 事件 - 当 - 鼠标按下时
        /// </summary>
        public UnityEvent eve_on_pointer_Press;
        /// <summary>
        /// 事件 - 当 - 鼠标进入时
        /// </summary>
        public UnityEvent eve_on_pointer_Enter;
        /// <summary>
        /// 事件 - 当 - 鼠标退出时
        /// </summary>
        public UnityEvent eve_on_pointer_Exit;
        /// <summary>
        /// 事件 - 当 - 鼠标抬起时
        /// </summary>
        public UnityEvent eve_on_pointer_Release;
        /// <summary>
        /// 事件 - 当 - 鼠标选中时
        /// </summary>
        public UnityEvent eve_on_pointer_Select;
        /// <summary>
        /// 事件 - 当 - 鼠标取消选中时
        /// </summary>
        public UnityEvent eve_on_pointer_Unselect;
        /// <summary>
        /// 事件 - 当 - 鼠标拖拽时
        /// </summary>
        public UnityEvent eve_on_pointer_Drag;
        /// <summary>
        /// 事件 - 当 - 鼠标拖放时
        /// </summary>
        public UnityEvent eve_on_pointer_Drop;

        public bool Enabled = true;

        public bool
            fold_param = true,
            fold_option = true,
            fold_based = true;

        void Start()
        {
            if (HudElement == null)
                HudElement = GetComponent<XHud_Module_Element>();
            if (TriggerImage == null)
                TriggerImage = GetComponent<Image>();
        }

        void Update()
        {

        }

        private void OnDisable()
        {
            if (!AutoClearActionsAndEvents)
                return;
            ClearAllEvents();
            ClearAllActions();
        }

        /// <summary>
        /// 清空所有事件
        /// </summary>
        public void ClearAllEvents()
        {
            eve_on_pointer_Clicked.RemoveAllListeners();
            eve_on_pointer_Press.RemoveAllListeners();
            eve_on_pointer_Enter.RemoveAllListeners();
            eve_on_pointer_Exit.RemoveAllListeners();
            eve_on_pointer_Release.RemoveAllListeners();
            eve_on_pointer_Select.RemoveAllListeners();
            eve_on_pointer_Unselect.RemoveAllListeners();
            eve_on_pointer_Drag.RemoveAllListeners();
            eve_on_pointer_Drop.RemoveAllListeners();
        }

        /// <summary>
        /// 清空所有委托动作
        /// </summary>
        public void ClearAllActions()
        {
            act_on_Clicked = null;
            act_on_Press = null;
            act_on_Enter = null;
            act_on_Exit = null;
            act_on_Release = null;
            act_on_Select = null;
            act_on_Unselect = null;
            act_on_Drag = null;
            act_on_Drop = null;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (!Enabled) return;

            base.OnPointerEnter(eventData);
            if (act_on_Enter != null)
                act_on_Enter(HudElement, eventData);
            eve_on_pointer_Enter.Invoke();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (!Enabled) return;

            base.OnPointerExit(eventData);
            if (act_on_Enter != null)
                act_on_Enter(HudElement, eventData);
            eve_on_pointer_Enter.Invoke();
        }

        public override void OnSelect(BaseEventData eventData)
        {
            if (!Enabled) return;

            base.OnSelect(eventData);
            if (act_on_Select != null)
                act_on_Select(HudElement, eventData);
            eve_on_pointer_Select.Invoke();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            if (!Enabled) return;

            base.OnDeselect(eventData);
            if (act_on_Unselect != null)
                act_on_Unselect(HudElement, eventData);
            eve_on_pointer_Unselect.Invoke();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!Enabled) return;

            base.OnPointerClick(eventData);
            if (act_on_Clicked != null)
                act_on_Clicked(HudElement, eventData);
            eve_on_pointer_Clicked.Invoke();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!Enabled) return;

            base.OnPointerDown(eventData);
            if (act_on_Press != null)
                act_on_Press(HudElement, eventData);
            eve_on_pointer_Press.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!Enabled) return;

            base.OnPointerUp(eventData);
            if (act_on_Release != null)
                act_on_Release(HudElement, eventData);
            eve_on_pointer_Release.Invoke();
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (!Enabled) return;

            base.OnDrag(eventData);
            if (act_on_Drag != null)
                act_on_Drag(HudElement, eventData);
            eve_on_pointer_Drag.Invoke();
        }

        public override void OnDrop(PointerEventData eventData)
        {
            if (!Enabled) return;

            base.OnDrop(eventData);
            if (act_on_Drop != null)
                act_on_Drop(HudElement, eventData);
            eve_on_pointer_Drop.Invoke();
        }
    }
}