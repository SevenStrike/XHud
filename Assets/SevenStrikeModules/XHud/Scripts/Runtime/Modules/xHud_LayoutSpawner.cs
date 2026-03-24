namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XHud.Enums;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    [System.Serializable]
    public class HudLayoutSpawnerParams
    {
        public string libname;
        public List<LayoutSpawnerItem> SpawnItemList = new List<LayoutSpawnerItem>();
        public string SpawnerIndicator;

        public Motion_Creator CreateArgs;
        public string CreateParamName;
        public string Crc_Lib_Name;

        public Motion_Recycler RecycleArgs;
        public string RecycleParamName;
        public string Rec_Lib_Name;

        public KeyCode Key_Create = KeyCode.F7;
        public KeyCode Key_Recycle = KeyCode.F8;

        public bool UseManullyKey;
        public bool IsSpawning;

        public bool create_fold_move;
        public bool create_fold_rotate;
        public bool create_fold_alpha;
        public bool recycle_fold_move;
        public bool recycle_fold_rotate;
        public bool recycle_fold_alpha;
    }

    [System.Serializable]
    public class LayoutSpawnerItem
    {
        /// <summary>
        /// 生成的元素名称
        /// </summary>
        public string SpawnName;
        /// <summary>
        /// 标识名称
        /// </summary>
        public string Indicator;
        /// <summary>
        /// 生成延迟时间
        /// </summary>
        public float Delay_Spawn;
        /// <summary>
        /// 回收延迟时间
        /// </summary>
        public float Delay_Despawn;
        /// <summary>
        /// 生成的位置
        /// </summary>
        public Vector3 Position = Vector3.one;
        /// <summary>
        /// 生成的角度
        /// </summary>
        public Vector3 Euler = Vector3.zero;
        /// <summary>
        /// 生成的偏移
        /// </summary>
        public Vector3 Offset = Vector3.zero;
        /// <summary>
        /// 生成的缩放
        /// </summary>
        public Vector3 Scale = Vector3.one;
        /// <summary>
        /// 生成的尺寸
        /// </summary>
        public Vector2 Size;
        /// <summary>
        /// 生成的轴心点
        /// </summary>
        public Vector2 Pivot;
        /// <summary>
        /// 生成的最小锚点
        /// </summary>
        public Vector2 Anchor_Min;
        /// <summary>
        /// 生成的最大锚点
        /// </summary>
        public Vector2 Anchor_Max;
        /// <summary>
        /// 生成的元素是否自动激活 ElementIn
        /// </summary>
        public string AutoIn = "自动 In";
        /// <summary>
        /// 是否正在运动中
        /// </summary>
        public bool InMotion;
        /// <summary>
        /// 是否已生成
        /// </summary>
        public bool Spawned;
        /// <summary>
        /// 生成的元素节点
        /// </summary>
        public HudElementNode SpawnedElementNode;
        /// <summary>
        /// 使用全局动效
        /// </summary>
        public string UseSpawnerMotion = "生成器动效";
        /// <summary>
        /// 生成动效参数
        /// </summary>
        public Motion_Creator CreateArgs = new Motion_Creator();
        /// <summary>
        /// 回收动效参数
        /// </summary>
        public Motion_Recycler RecycleArgs = new Motion_Recycler();
        /// <summary>
        /// 动效运动进度条
        /// </summary>
        public float MotionPercentage;
        /// <summary>
        /// 是否折叠
        /// </summary>
        public bool isFold = true;
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool isEnabled = true;
        public UnityAction<xHud_Module_Element> act_on_element_in_start;
        public UnityAction<xHud_Module_Element> act_on_element_in_end;
        public UnityAction<xHud_Module_Element> act_on_element_out_start;
        public UnityAction<xHud_Module_Element> act_on_element_out_end;
    }

    public class xHud_LayoutSpawner : MonoBehaviour
    {
        public string LibName;

        /// <summary>
        /// 编辑器下的已装载UI元素指示器 - 屏幕级别
        /// </summary>
        public bool IsLoadedLayout_Screen;
        /// <summary>
        /// 编辑器下的已装载UI元素指示器 - 世界级别
        /// </summary>
        public bool IsLoadedLayout_World;

        public List<LayoutSpawnerItem> SpawnItemList_Screen = new List<LayoutSpawnerItem>();
        public List<LayoutSpawnerItem> SpawnItemList_World = new List<LayoutSpawnerItem>();

        public string SpawnerIndicator;

        public Motion_Creator CreateArgs;
        public string CreateParamName;
        public string Crc_Lib_Name;

        public Motion_Recycler RecycleArgs;
        public string RecycleParamName;
        public string Rec_Lib_Name;

        public SpawnFunctionKey SpawnFunctionKey_Primary = SpawnFunctionKey.Shift;
        public SpawnFunctionKey SpawnFunctionKey_Secondary = SpawnFunctionKey.None;

        public KeyCode Key_Create = KeyCode.F7;
        public KeyCode Key_Recycle = KeyCode.F8;

        public bool UseManullyKey;

        public bool ControlScreen = true;
        public bool ControlWorld = true;

        public bool FoldScreen = true;
        public bool FoldWorld = true;

        /// <summary>
        /// 防止元素动效未结束时再次操作生成或回收
        /// </summary>
        public bool ProtectedAction = true;

        [SerializeField]
        /// <summary>
        /// 所有动效播放状态
        /// </summary>
        private bool inMotion = false;
        public bool InMotion
        {
            get
            {
                return inMotion;
            }
            set
            {
                if (value != inMotion)
                {
                    inMotion = value;
                    if (act_on_motion_state_changed != null)
                        act_on_motion_state_changed(value);
                }
            }
        }

        /// <summary>
        /// 列表元素生成间隔延迟时间 - 屏幕
        /// </summary>
        public float DelayOrder_Spawn_Screen = 0;
        /// <summary>
        /// 列表元素生成间隔延迟时间 - 世界
        /// </summary>
        public float DelayOrder_Spawn_World = 0;
        /// <summary>
        /// 列表元素回收间隔延迟时间 - 屏幕
        /// </summary>
        public float DelayOrder_Despawn_Screen = 0;
        /// <summary>
        /// 列表元素回收间隔延迟时间 - 世界
        /// </summary>
        public float DelayOrder_Despawn_World = 0;

        public bool create_fold_move;
        public bool create_fold_rotate;
        public bool create_fold_alpha;
        public bool recycle_fold_move;
        public bool recycle_fold_rotate;
        public bool recycle_fold_alpha;

        /// <summary>
        /// 动作 - 生成元素
        /// </summary>
        public UnityAction<HudSpace> act_on_spawn;
        /// <summary>
        /// 动作 - 回收元素
        /// </summary>
        public UnityAction<HudSpace> act_on_despawn;
        /// <summary>
        /// 动作 - 元素动效状态改变
        /// </summary>
        public UnityAction<bool> act_on_motion_state_changed;

        private void Start()
        {
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {

        }

        private void Update()
        {
            if (!ProtectedActionState())
                if (FunctionKey_Primary_Detect() && FunctionKey_Secondary_Detect())
                {
                    if (Input.GetKeyDown(Key_Create))
                    {
                        if (!UseManullyKey)
                            return;
                        if (ControlScreen)
                        {
                            Spawn(HudSpace.屏幕空间);
                        }
                        if (ControlWorld)
                        {
                            Spawn(HudSpace.世界空间);
                        }
                    }

                    if (Input.GetKeyDown(Key_Recycle))
                    {
                        if (!UseManullyKey)
                            return;
                        if (ControlScreen)
                        {
                            Despawn(HudSpace.屏幕空间);
                        }
                        if (ControlWorld)
                        {
                            Despawn(HudSpace.世界空间);
                        }
                    }
                }
        }

        #region 调用方法
        /// <summary>
        /// 根据空间类型生成元素
        /// </summary>
        /// <param tweenName="space"></param>
        public void Spawn(HudSpace space)
        {
            if (!UseManullyKey)
                return;

            switch (space)
            {
                case HudSpace.屏幕空间:
                    if (DelayOrder_Spawn_Screen > 0)
                    {
                        //顺序生成 - 屏幕
                        hsp_Screen_OrderDelay_Spawn();
                    }
                    else
                    {
                        //并发生成 - 屏幕
                        hsp_Screen_Spawn();
                    }
                    break;
                case HudSpace.世界空间:
                    if (DelayOrder_Spawn_World > 0)
                    {
                        //顺序生成 - 世界
                        hsp_World_OrderDelay_Spawn();
                    }
                    else
                    {
                        //并发生成 - 世界
                        hsp_World_Spawn();
                    }
                    break;
            }
        }

        /// <summary>
        /// 根据空间类型回收元素
        /// </summary>
        /// <param tweenName="space"></param>
        public void Despawn(HudSpace space)
        {
            switch (space)
            {
                case HudSpace.屏幕空间:
                    if (DelayOrder_Despawn_Screen > 0)
                    {
                        //顺序回收 - 屏幕
                        hsp_Screen_OrderDelay_Despawn();
                    }
                    else
                    {
                        //并发回收 - 屏幕
                        hsp_Screen_Despawn();
                    }
                    break;
                case HudSpace.世界空间:
                    if (DelayOrder_Despawn_World > 0)
                    {
                        //顺序回收 - 世界
                        hsp_World_OrderDelay_Despawn();
                    }
                    else
                    {
                        //并发回收 - 世界
                        hsp_World_Despawn();
                    }
                    break;
            }
        }
        #endregion

        #region Screen 生成 / 回收 - 并发
        /// <summary>
        /// 从池中取出Hud元素 - 屏幕级别 - 并发生成
        /// </summary>
        private void hsp_Screen_Spawn()
        {
            if (CheckElementLibrary_IsEmpty())
                return;

            for (int i = 0; i < SpawnItemList_Screen.Count; i++)
            {
                LayoutSpawnerItem item = SpawnItemList_Screen[i];
                if (!item.InMotion && !item.Spawned)
                    StartCoroutine(hsp_Create(item, HudSpace.屏幕空间));
            }
            if (act_on_spawn != null)
                act_on_spawn(HudSpace.屏幕空间);
        }

        /// <summary>
        /// 回收Hud元素到池中 - 屏幕级别 - 并发回收
        /// </summary>
        private void hsp_Screen_Despawn()
        {
            if (CheckElementLibrary_IsEmpty())
                return;

            for (int i = 0; i < SpawnItemList_Screen.Count; i++)
            {
                LayoutSpawnerItem item = SpawnItemList_Screen[i];
                if (!item.InMotion && item.Spawned)
                    StartCoroutine(hsp_Recycle(item));
            }
            if (act_on_despawn != null)
                act_on_despawn(HudSpace.屏幕空间);
        }
        #endregion

        #region World 生成 / 回收 - 并发
        /// <summary>
        /// 从池中取出Hud元素 - 世界级别 - 并发生成
        /// </summary>
        private void hsp_World_Spawn()
        {
            if (CheckElementLibrary_IsEmpty())
                return;

            for (int i = 0; i < SpawnItemList_World.Count; i++)
            {
                LayoutSpawnerItem item = SpawnItemList_World[i];
                if (!item.InMotion && !item.Spawned)
                    StartCoroutine(hsp_Create(item, HudSpace.世界空间));
            }
            if (act_on_spawn != null)
                act_on_spawn(HudSpace.世界空间);
        }

        /// <summary>
        /// 回收Hud元素到池中 - 世界级别 - 并发回收
        /// </summary>
        private void hsp_World_Despawn()
        {
            if (CheckElementLibrary_IsEmpty())
                return;

            for (int i = 0; i < SpawnItemList_World.Count; i++)
            {
                LayoutSpawnerItem item = SpawnItemList_World[i];
                if (!item.InMotion && item.Spawned)
                    StartCoroutine(hsp_Recycle(item));
            }
            if (act_on_despawn != null)
                act_on_despawn(HudSpace.世界空间);
        }
        #endregion

        #region Screen 生成 / 回收 - 顺序
        /// <summary>
        /// 从池中取出Hud元素 - 屏幕级别 - 顺序生成
        /// </summary>
        private void hsp_Screen_OrderDelay_Spawn()
        {
            if (CheckElementLibrary_IsEmpty())
                return;
            StartCoroutine(hsp_OrderDelay_Create(HudSpace.屏幕空间));
            if (act_on_spawn != null)
                act_on_spawn(HudSpace.屏幕空间);
        }

        /// <summary>
        /// 回收Hud元素到池中 - 屏幕级别 - 顺序回收
        /// </summary>
        private void hsp_Screen_OrderDelay_Despawn()
        {
            if (CheckElementLibrary_IsEmpty())
                return;
            StartCoroutine(hsp_OrderDelay_Recycle(HudSpace.屏幕空间));
            if (act_on_despawn != null)
                act_on_despawn(HudSpace.屏幕空间);
        }
        #endregion

        #region World 生成 / 回收 - 顺序
        /// <summary>
        /// 从池中取出Hud元素 - 世界级别 - 顺序生成
        /// </summary>
        private void hsp_World_OrderDelay_Spawn()
        {
            if (CheckElementLibrary_IsEmpty())
                return;
            StartCoroutine(hsp_OrderDelay_Create(HudSpace.世界空间));
            if (act_on_spawn != null)
                act_on_spawn(HudSpace.世界空间);
        }

        /// <summary>
        /// 回收Hud元素到池中 - 世界级别 - 顺序回收
        /// </summary>
        private void hsp_World_OrderDelay_Despawn()
        {
            if (CheckElementLibrary_IsEmpty())
                return;
            StartCoroutine(hsp_OrderDelay_Recycle(HudSpace.世界空间));
            if (act_on_despawn != null)
                act_on_despawn(HudSpace.世界空间);
        }
        #endregion

        //---------------------- 主要修改逻辑在这

        #region 并发生成 / 回收
        /// <summary>
        /// 生成
        /// </summary>
        /// <param tweenName="item"></param>
        /// <param tweenName="space"></param>
        /// <returns></returns>
        private IEnumerator hsp_Create(LayoutSpawnerItem item, HudSpace space)
        {
            //---延迟创建
            yield return new WaitForSeconds(item.Delay_Spawn);
            //---生成元素
            switch (space)
            {
                case HudSpace.屏幕空间:

                    item.SpawnedElementNode = xHud_Manager.Instance.hm_HudElement_Create_Screen(
                        LibName,
                        item.Indicator,
                        item.SpawnName,
                        item.Position + item.Offset,
                        item.Scale,
                        item.Size,
                        false,
                        null,
                        item.UseSpawnerMotion == "自身动效" ? item.CreateArgs : CreateArgs,
                       (element) =>/*动作委托：元素 In 开始*/
                       {
                           item.InMotion = true;

                           item.Spawned = true;

                           if (item.act_on_element_in_start != null)
                           {
                               item.act_on_element_in_start(element);
                           }
                       },
                       (progress) =>/*动作委托：元素 In 进度*/
                       {
                           item.MotionPercentage = progress;
                       },
                       (element) =>/*动作委托：元素 In 结束*/
                       {
                           item.InMotion = false;

                           if (item.act_on_element_in_end != null)
                           {
                               item.act_on_element_in_end(element);
                           }
                       },
                       (element) =>/*动作委托：元素 Out 开始*/
                       {
                           item.InMotion = true;

                           if (item.act_on_element_out_start != null)
                           {
                               item.act_on_element_out_start(element);
                           };
                       },
                       (progress) =>/*动作委托：元素 Out 进度*/
                       {
                           item.MotionPercentage = progress;
                       },
                       (element) =>/*动作委托：元素 Out 结束*/
                       {
                           item.InMotion = false;

                           item.Spawned = false;

                           if (item.act_on_element_out_end != null)
                           {
                               item.act_on_element_out_end(element);
                           };
                           item.SpawnedElementNode = null;
                       },
                        item.AutoIn == "自动 In" ? true : false);
                    break;
                case HudSpace.世界空间:

                    item.SpawnedElementNode = xHud_Manager.Instance.hm_HudElement_Create_World(
                        LibName,
                        item.Indicator,
                        item.SpawnName,
                        item.Size,
                        item.Position,
                        item.Euler,
                        item.Scale,
                        item.Offset,
                        item.UseSpawnerMotion == "自身动效" ? item.CreateArgs : CreateArgs,
                        (element) =>/*动作委托：元素 In 开始*/
                        {
                            item.InMotion = true;

                            item.Spawned = true;

                            if (item.act_on_element_in_start != null)
                            {
                                item.act_on_element_in_start(element);
                            }
                        },
                        (progress) =>/*动作委托：元素 In  进度*/
                        {
                            item.MotionPercentage = progress;
                        },
                        (element) =>/*动作委托：元素 In 结束*/
                        {
                            item.InMotion = false;

                            if (item.act_on_element_in_end != null)
                            {
                                item.act_on_element_in_end(element);
                            }
                        },
                        (element) =>/*动作委托：元素 Out 开始*/
                        {
                            item.InMotion = true;

                            if (item.act_on_element_out_start != null)
                            {
                                item.act_on_element_out_start(element);
                            };
                        },
                        (progres) =>/*动作委托：元素 Out 进度*/
                        {
                            item.MotionPercentage = 1 - progres;
                        },
                        (element) =>/*动作委托：元素 Out 结束*/
                        {
                            item.InMotion = false;

                            item.Spawned = false;

                            if (item.act_on_element_out_end != null)
                            {
                                item.act_on_element_out_end(element);
                            };
                            item.SpawnedElementNode = null;
                        },
                        item.AutoIn == "自动 In" ? true : false);
                    break;
            }
        }

        /// <summary>
        /// 回收
        /// </summary>
        /// <param tweenName="item"></param>
        /// <returns></returns>
        private IEnumerator hsp_Recycle(LayoutSpawnerItem item)
        {
            //---延迟回收
            yield return new WaitForSeconds(item.Delay_Despawn);
            //---回收元素
            xHud_Manager.Instance.hm_HudElement_RecycleAt(
                item.SpawnedElementNode.Element,
                item.UseSpawnerMotion == "自身动效" ? item.RecycleArgs : RecycleArgs,
                (element) =>/*动作委托：元素 Out 开始*/
                {
                    item.InMotion = true;

                    if (item.act_on_element_out_start != null)
                    {
                        item.act_on_element_out_start(element);
                    };
                },
                 (progres) =>/*动作委托：元素 Out 进度*/
                 {
                     item.MotionPercentage = 1 - progres;
                 },
                (element) =>/*动作委托：元素 Out 结束*/
                {
                    item.InMotion = false;

                    item.Spawned = false;

                    if (item.act_on_element_out_end != null)
                    {
                        item.act_on_element_out_end(element);
                    };
                    item.SpawnedElementNode = null;
                });
            yield return null;
        }
        #endregion

        #region 顺序生成 / 回收
        /// <summary>
        /// 生成 - 按列表顺序延迟模式
        /// </summary>
        /// <param tweenName="space"></param>
        /// <returns></returns>
        private IEnumerator hsp_OrderDelay_Create(HudSpace space)
        {
            //---生成元素
            switch (space)
            {
                case HudSpace.屏幕空间:
                    for (int i = 0; i < SpawnItemList_Screen.Count; i++)
                    {
                        LayoutSpawnerItem item = SpawnItemList_Screen[i];
                        if (!item.InMotion && !item.Spawned)
                        {
                            //---延迟创建
                            yield return new WaitForSeconds(item.Delay_Spawn + DelayOrder_Spawn_Screen);
                            item.SpawnedElementNode = xHud_Manager.Instance.hm_HudElement_Create_Screen(
                                LibName,
                                item.Indicator,
                                item.SpawnName,
                                item.Position + item.Offset,
                                item.Scale,
                                item.Size,
                                false,
                                null,
                                item.UseSpawnerMotion == "自身动效" ? item.CreateArgs : CreateArgs,
                                (element) =>/*动作委托：元素 In 开始*/
                                {
                                    item.InMotion = true;

                                    item.Spawned = true;

                                    if (item.act_on_element_in_start != null)
                                    {
                                        item.act_on_element_in_start(element);
                                    }
                                },
                                (progress) =>/*动作委托：元素In  进度*/
                                {
                                    item.MotionPercentage = progress;
                                },
                                (element) =>/*动作委托：元素 In 结束*/
                                {
                                    item.InMotion = false;

                                    if (item.act_on_element_in_end != null)
                                    {
                                        item.act_on_element_in_end(element);
                                    }
                                },
                                (element) =>/*动作委托：元素 Out 开始*/
                                {
                                    item.InMotion = true;

                                    if (item.act_on_element_out_start != null)
                                    {
                                        item.act_on_element_out_start(element);
                                    };
                                },
                                (progres) =>/*动作委托：元素 Out 进度*/
                                {
                                    item.MotionPercentage = 1 - progres;
                                },
                                (element) =>/*动作委托：元素 Out 结束*/
                                {
                                    item.InMotion = false;

                                    item.Spawned = false;

                                    if (item.act_on_element_out_end != null)
                                    {
                                        item.act_on_element_out_end(element);
                                    };
                                    item.SpawnedElementNode = null;
                                },
                                item.AutoIn == "自动 In" ? true : false);
                        }
                    }
                    break;
                case HudSpace.世界空间:
                    for (int i = 0; i < SpawnItemList_World.Count; i++)
                    {
                        LayoutSpawnerItem item = SpawnItemList_World[i];
                        if (!item.InMotion && !item.Spawned)
                        {
                            //---延迟创建
                            yield return new WaitForSeconds(item.Delay_Spawn + DelayOrder_Spawn_World);
                            item.SpawnedElementNode = xHud_Manager.Instance.hm_HudElement_Create_World(
                                LibName,
                                item.Indicator,
                                item.SpawnName,
                                item.Size,
                                item.Position,
                                item.Euler,
                                item.Scale,
                                item.Offset,
                                item.UseSpawnerMotion == "自身动效" ? item.CreateArgs : CreateArgs,
                                (element) =>/*动作委托：元素 In 开始*/
                                {
                                    item.InMotion = true;

                                    item.Spawned = true;

                                    if (item.act_on_element_in_start != null)
                                    {
                                        item.act_on_element_in_start(element);
                                    }
                                },
                                (progress) =>/*动作委托：元素 In 进度*/
                                {
                                    item.MotionPercentage = progress;
                                },
                                (element) =>/*动作委托：元素 In 结束*/
                                {
                                    item.InMotion = false;

                                    if (item.act_on_element_in_end != null)
                                    {
                                        item.act_on_element_in_end(element);
                                    }
                                },
                                (element) =>/*动作委托：元素 Out 开始*/
                                {
                                    item.InMotion = true;

                                    if (item.act_on_element_out_start != null)
                                    {
                                        item.act_on_element_out_start(element);
                                    };
                                },
                                 (progres) =>/*动作委托：元素 Out 进度*/
                                 {
                                     item.MotionPercentage = 1 - progres;
                                 },
                                (element) =>/*动作委托：元素 Out 结束*/
                                {
                                    item.InMotion = false;

                                    item.Spawned = false;

                                    if (item.act_on_element_out_end != null)
                                    {
                                        item.act_on_element_out_end(element);
                                    };
                                    item.SpawnedElementNode = null;
                                },
                                item.AutoIn == "自动 In" ? true : false);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 回收 - 按列表顺序延迟模式
        /// </summary>
        /// <param tweenName="space"></param>
        /// <returns></returns>
        private IEnumerator hsp_OrderDelay_Recycle(HudSpace space)
        {
            switch (space)
            {
                case HudSpace.屏幕空间:
                    for (int i = 0; i < SpawnItemList_Screen.Count; i++)
                    {
                        LayoutSpawnerItem item = SpawnItemList_Screen[i];

                        if (!item.InMotion && item.Spawned)
                        {
                            //---延迟回收
                            yield return new WaitForSeconds(item.Delay_Despawn + DelayOrder_Despawn_Screen);

                            //---回收元素
                            xHud_Manager.Instance.hm_HudElement_RecycleAt(
                                item.SpawnedElementNode.Element,
                                item.UseSpawnerMotion == "自身动效" ? item.RecycleArgs : RecycleArgs,
                                (element) =>/*动作委托：元素 Out 开始*/
                                {
                                    item.InMotion = true;

                                    if (item.act_on_element_out_start != null)
                                    {
                                        item.act_on_element_out_start(element);
                                    };
                                },
                                 (progres) =>/*动作委托：元素 Out 进度*/
                                 {
                                     item.MotionPercentage = 1 - progres;
                                 },
                                (element) =>/*动作委托：元素 Out 结束*/
                                {
                                    item.InMotion = false;

                                    item.Spawned = false;

                                    if (item.act_on_element_out_end != null)
                                    {
                                        item.act_on_element_out_end(element);
                                    };
                                    item.SpawnedElementNode = null;
                                });
                        }
                    }
                    break;
                case HudSpace.世界空间:
                    for (int i = 0; i < SpawnItemList_World.Count; i++)
                    {
                        LayoutSpawnerItem item = SpawnItemList_World[i];

                        if (!item.InMotion && item.Spawned)
                        {
                            //---延迟回收
                            yield return new WaitForSeconds(item.Delay_Despawn + DelayOrder_Despawn_World);

                            //---回收元素
                            xHud_Manager.Instance.hm_HudElement_RecycleAt(
                                item.SpawnedElementNode.Element,
                                item.UseSpawnerMotion == "自身动效" ? item.RecycleArgs : RecycleArgs,
                                (element) =>/*动作委托：元素 Out 开始*/
                                {
                                    item.InMotion = true;

                                    if (item.act_on_element_out_start != null)
                                    {
                                        item.act_on_element_out_start(element);
                                    };
                                },
                                 (progres) =>/*动作委托：元素 Out 进度*/
                                 {
                                     item.MotionPercentage = 1 - progres;
                                 },
                                (element) =>/*动作委托：元素 Out 结束*/
                                {
                                    item.InMotion = false;

                                    item.Spawned = false;

                                    if (item.act_on_element_out_end != null)
                                    {
                                        item.act_on_element_out_end(element);
                                    };
                                    item.SpawnedElementNode = null;
                                });
                        }
                    }
                    break;
            }
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 检查判断元素库不是空的
        /// </summary>
        /// <returns></returns>
        private bool CheckElementLibrary_IsEmpty()
        {
            bool isEmpty = false;

            if (xHud_Manager.Instance.Hud_ElementLibrarys == null)
                isEmpty = true;

            return isEmpty;
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

        /// <summary>
        /// 检测所有动效是否正在播放
        /// </summary>
        /// <returns></returns>
        private bool ProtectedActionState()
        {
            if (!ProtectedAction)
            {
                InMotion = false;
                return false;
            }

            bool sw = false;

            for (int i = 0; i < SpawnItemList_Screen.Count; i++)
            {
                if (SpawnItemList_Screen[i].InMotion)
                {
                    sw = true;
                    break;
                }
            }

            for (int i = 0; i < SpawnItemList_World.Count; i++)
            {
                if (SpawnItemList_World[i].InMotion)
                {
                    sw = true;
                    break;
                }
            }

            InMotion = sw;

            return sw;
        }
        #endregion
    }
}