namespace SevenStrikeModules.XHud
{
    using UnityEngine;
    using UnityEngine.Events;

    public enum SpawnFunctionKey
    {
        None = 0,
        Ctrl = 1,
        Alt = 2,
        Shift = 3,
    }

    public class XHud_Spawner : MonoBehaviour
    {
        public string LibName;

        public string SpawnName;
        public string SpawnIndicator;

        public string SpawnerIndicator;
        public XHud_Module_Element SpawnElement;
        public Motion_Creator CreateArgs;
        public string CreateParamName;

        public Motion_Recycler RecycleArgs;
        public string RecycleParamName;

        public Vector2 ElementSize;
        public Vector3 ElementOffset;
        public Vector3 ElementScale = new Vector3(1, 1, 1);
        public Vector3 WorldPosition;
        public Vector3 WorldRotation;
        public Vector3 WorldScale = new Vector3(1, 1, 1);

        public Transform ReferObject;

        public SpawnFunctionKey SpawnFunctionKey_Primary = SpawnFunctionKey.Shift;
        public SpawnFunctionKey SpawnFunctionKey_Secondary = SpawnFunctionKey.None;

        public KeyCode Key_Create = KeyCode.S;
        public KeyCode Key_Recycle = KeyCode.D;

        public bool Opt_ManullyCreate = true;
        public bool Opt_VisuallerCreate;
        public bool Opt_WorldCreate;

        [SerializeField]
        private bool opt_IsRunning;
        public bool Opt_IsRunning
        {
            get
            {
                return opt_IsRunning;
            }
            set
            {
                if (value != opt_IsRunning)
                {
                    opt_IsRunning = value;

                    if (act_on_element_state_changed != null)
                        act_on_element_state_changed(value);

                    if (eve_on_element_state_changed != null)
                    {
                        eve_on_element_state_changed.Invoke(value);
                    }
                }
            }
        }
        public bool Opt_RMSEnabled;
        public bool Opt_AutoIn = true;

        public string Crc_Lib_Name;
        public string Rec_Lib_Name;
        public string RMS_SelctedName;

        public bool recycle_fold_move;
        public bool recycle_fold_rotate;
        public bool recycle_fold_alpha;
        public bool create_fold_move;
        public bool create_fold_rotate;
        public bool create_fold_alpha;

        /// <summary>
        /// 防止元素动效未结束时再次操作生成或回收
        /// </summary>
        public bool ProtectedAction = true;

        /// <summary>
        /// 动作 - 生成元素
        /// </summary>
        public UnityAction<XHud_Module_Element> act_on_element_spawn;
        /// <summary>
        /// 动作 - 回收元素
        /// </summary>
        public UnityAction<XHud_Module_Element> act_on_element_despawn;

        /// <summary>
        /// 动作 - 生成元素 - 进入 - 开始
        /// </summary>
        public UnityAction act_on_element_in_start;
        /// <summary>
        /// 动作 - 生成元素 - 进入 - 结束
        /// </summary>
        public UnityAction act_on_element_in_end;
        /// <summary>
        /// 动作 - 生成元素 - 退出 - 开始
        /// </summary>
        public UnityAction act_on_element_out_start;
        /// <summary>
        /// 动作 - 生成元素 - 退出 - 结束
        /// </summary>
        public UnityAction act_on_element_out_end;
        /// <summary>
        /// 动作 - 元素 - 生成 - 状态
        /// </summary>
        public UnityAction<bool> act_on_element_state_changed;

        /// <summary>
        /// 动作 - 生成元素开始
        /// </summary>
        public UnityEvent eve_on_element_spawn_start;
        /// <summary>
        /// 动作 - 生成元素结束
        /// </summary>
        public UnityEvent eve_on_element_spawn_end;
        /// <summary>
        /// 动作 - 回收元素开始
        /// </summary>
        public UnityEvent eve_on_element_despawn_start;
        /// <summary>
        /// 动作 - 回收元素结束
        /// </summary>
        public UnityEvent eve_on_element_despawn_end;
        /// <summary>
        /// 动作 - 元素 - 生成 - 状态
        /// </summary>
        public UnityEvent<bool> eve_on_element_state_changed;


        private void Start()
        {

        }

        private void OnEnable()
        {
            if (Opt_VisuallerCreate)
            {
                if (XHud_Manager.Instance == null)
                    return;
                if (CheckElementLibraryIsEmpty())
                    return;
                if (SpawnElement == null)
                    SpawnElement = hsp_Spawn();
            }
        }

        private void OnDisable()
        {
            if (Opt_VisuallerCreate)
            {
                if (CheckElementLibraryIsEmpty())
                    return;
                if (SpawnElement != null)
                    hsp_Despawn(RecycleArgs);
            }
        }

        private void Update()
        {
            if (FunctionKey_Primary_Detect() && FunctionKey_Secondary_Detect())
            {
                if (Input.GetKeyDown(Key_Create))
                {
                    if (CheckElementLibraryIsEmpty())
                        return;
                    if (!Opt_ManullyCreate)
                        return;
                    if (Opt_IsRunning)
                        return;
                    if (SpawnElement != null)
                        return;
                    SpawnElement = hsp_Spawn(SpawnIndicator, CreateArgs);
                }

                if (Input.GetKeyDown(Key_Recycle))
                {
                    if (CheckElementLibraryIsEmpty())
                        return;
                    if (!Opt_ManullyCreate)
                        return;
                    if (Opt_IsRunning)
                        return;
                    if (SpawnElement == null)
                        return;
                    hsp_Despawn(RecycleArgs);
                }
            }
        }

        /// <summary>
        /// 从池中取出一个Hud元素
        /// </summary>
        /// <param tweenName="IndicatorName">标识名称，如果不填则使用脚本自带的标识名称</param>
        /// <param tweenName="CreateParam">创建参数，如果不填则使用脚本自带的创建参数</param>
        /// <returns></returns>
        public XHud_Module_Element hsp_Spawn(string IndicatorName = "", Motion_Creator CreateParam = null)
        {
            XHud_Module_Element element = null;

            ///---如果UI渲染模式为世界空间则使用世界空间专用的方法
            if (Opt_WorldCreate)
            {
                ///---如果参考物体存在则生成的UI的坐标信息则参考这个物体的坐标信息
                if (ReferObject != null)
                {
                    element = XHud_Manager.Instance.hm_HudElement_Create_World(LibName, string.IsNullOrEmpty(IndicatorName) ? SpawnIndicator : IndicatorName, SpawnName, ElementSize, ReferObject.position, ReferObject.eulerAngles, ReferObject.localScale, ElementOffset, CreateParam == null ? this.CreateArgs : CreateParam, null, (wrap) =>
                    {
                        ///--------当元素 - 进入 - 开始时
                        if (act_on_element_in_start != null)
                            act_on_element_in_start();
                        Opt_IsRunning = true;
                    }, null, (wrap) =>
                     {
                         ///--------当元素 - 进入 - 结束时
                         if (act_on_element_in_end != null)
                             act_on_element_in_end();
                         Opt_IsRunning = false;
                     }, null, null).Element;
                }
                else
                {
                    element = XHud_Manager.Instance.hm_HudElement_Create_World(LibName, string.IsNullOrEmpty(IndicatorName) ? SpawnIndicator : IndicatorName, SpawnName, ElementSize, WorldPosition, WorldRotation, WorldScale, ElementOffset, CreateParam == null ? this.CreateArgs : CreateParam, null, (wrap) =>
                    {
                        ///--------当元素 - 进入 - 开始时
                        if (act_on_element_in_start != null)
                            act_on_element_in_start();
                        Opt_IsRunning = true;
                    }, null, (wrap) =>
                      {
                          ///--------当元素 - 进入 - 结束时
                          if (act_on_element_in_end != null)
                              act_on_element_in_end();
                          Opt_IsRunning = false;
                      }, null, null).Element;
                }
            }
            else
            {
                element = XHud_Manager.Instance.hm_HudElement_Create_Screen(LibName, string.IsNullOrEmpty(IndicatorName) ? SpawnIndicator : IndicatorName, SpawnName, ElementOffset, ElementScale, ElementSize, Opt_WorldCreate ? false : Opt_RMSEnabled, RMS_SelctedName, CreateParam == null ? this.CreateArgs : CreateParam, (wrap) =>
                {
                    ///--------当元素 - 进入 - 开始时
                    if (act_on_element_in_start != null)
                        act_on_element_in_start();
                    eve_on_element_spawn_start.Invoke();
                    Opt_IsRunning = true;
                }, null, (wrap) =>
                {
                    ///--------当元素 - 进入 - 结束时
                    if (act_on_element_in_end != null)
                        act_on_element_in_end();
                    eve_on_element_spawn_end.Invoke();
                    Opt_IsRunning = false;
                }, null, null, null, Opt_AutoIn).Element;
            }

            if (act_on_element_spawn != null)
                act_on_element_spawn(element);
            SpawnElement = element;
            return element;
        }

        /// <summary>
        /// 回收一个Hud元素到池中
        /// </summary>
        /// <param tweenName="RecycleParam">回收参数</param>
        /// <param tweenName="actionstart">事件 - 回收开始</param>
        /// <param tweenName="actionend">事件 - 回收结束</param>
        public void hsp_Despawn(Motion_Recycler RecycleParam = null, UnityAction<XHud_Module_Element> actionstart = null, UnityAction<XHud_Module_Element> actionend = null)
        {
            XHud_Manager.Instance.hm_HudElement_RecycleAt(SpawnElement, RecycleParam == null ? this.RecycleArgs : RecycleParam, actionstart == null ? (wrap) =>
            {
                if (act_on_element_out_start != null)
                    act_on_element_out_start();
                eve_on_element_despawn_start.Invoke();
                Opt_IsRunning = true;
            }
            : actionstart, null, actionend == null ? (wrap) =>
            {
                if (act_on_element_out_end != null)
                    act_on_element_out_end();
                if (act_on_element_despawn != null)
                    act_on_element_despawn(SpawnElement);
                eve_on_element_despawn_end.Invoke();
                SpawnElement = null;
                Opt_IsRunning = false;
            }
            : actionend);
        }

        /// <summary>
        /// 检查判断元素库不是空的
        /// </summary>
        /// <returns></returns>
        public bool CheckElementLibraryIsEmpty()
        {
            bool sw = false;
            if (!Application.isPlaying)
            {
                XHud_Manager mgr = FindFirstObjectByType<XHud_Manager>();

                if (mgr.Hud_ElementLibrarys == null)
                    sw = true;
                if (mgr.Hud_ElementLibrarys.Count <= 0)
                    sw = true;
            }
            else
            {
                if (XHud_Manager.Instance.Hud_ElementLibrarys == null)
                    sw = true;
                if (XHud_Manager.Instance.Hud_ElementLibrarys.Count <= 0)
                    sw = true;
            }
            return sw;
        }

        /// <summary>
        /// 第一功能按键组合检测
        /// </summary>
        /// <returns></returns>
        private bool FunctionKey_Primary_Detect()
        {
            bool ispress = false;
            switch (SpawnFunctionKey_Primary)
            {
                case SpawnFunctionKey.None:
                    ispress = true;
                    break;
                case SpawnFunctionKey.Ctrl:
                    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                        ispress = true;
                    break;
                case SpawnFunctionKey.Alt:
                    if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                        ispress = true;
                    break;
                case SpawnFunctionKey.Shift:
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                        ispress = true;
                    break;
            }
            return ispress;
        }

        /// <summary>
        /// 第二功能按键组合检测
        /// </summary>
        /// <returns></returns>
        private bool FunctionKey_Secondary_Detect()
        {
            bool ispress = false;
            switch (SpawnFunctionKey_Secondary)
            {
                case SpawnFunctionKey.None:
                    ispress = true;
                    break;
                case SpawnFunctionKey.Ctrl:
                    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                        ispress = true;
                    break;
                case SpawnFunctionKey.Alt:
                    if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                        ispress = true;
                    break;
                case SpawnFunctionKey.Shift:
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                        ispress = true;
                    break;
            }
            return ispress;
        }
    }
}