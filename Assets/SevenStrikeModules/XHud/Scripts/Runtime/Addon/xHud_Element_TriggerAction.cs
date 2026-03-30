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
            base.OnPointerEnter(eventData);
            if (act_on_Enter != null)
                act_on_Enter(HudElement, eventData);
            eve_on_pointer_Enter.Invoke();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (act_on_Enter != null)
                act_on_Enter(HudElement, eventData);
            eve_on_pointer_Enter.Invoke();
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            if (act_on_Select != null)
                act_on_Select(HudElement, eventData);
            eve_on_pointer_Select.Invoke();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            if (act_on_Unselect != null)
                act_on_Unselect(HudElement, eventData);
            eve_on_pointer_Unselect.Invoke();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (act_on_Clicked != null)
                act_on_Clicked(HudElement, eventData);
            eve_on_pointer_Clicked.Invoke();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (act_on_Press != null)
                act_on_Press(HudElement, eventData);
            eve_on_pointer_Press.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (act_on_Release != null)
                act_on_Release(HudElement, eventData);
            eve_on_pointer_Release.Invoke();
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            if (act_on_Drag != null)
                act_on_Drag(HudElement, eventData);
            eve_on_pointer_Drag.Invoke();
        }

        public override void OnDrop(PointerEventData eventData)
        {
            base.OnDrop(eventData);
            if (act_on_Drop != null)
                act_on_Drop(HudElement, eventData);
            eve_on_pointer_Drop.Invoke();
        }
    }
}