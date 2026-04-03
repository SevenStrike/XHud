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
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    [System.Serializable]
    public class XHud_LayoutSpawner_Args
    {
        public string libname;
        public List<XHud_LayoutSpawner_Item> SpawnItemList = new List<XHud_LayoutSpawner_Item>();
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
    public class XHud_LayoutSpawner_Item
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
        public UnityAction<XHud_Module_Element> act_on_element_in_start;
        public UnityAction<XHud_Module_Element> act_on_element_in_end;
        public UnityAction<XHud_Module_Element> act_on_element_out_start;
        public UnityAction<XHud_Module_Element> act_on_element_out_end;
    }

    public class XHud_LayoutSpawner : MonoBehaviour
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

        public List<XHud_LayoutSpawner_Item> SpawnItemList_Screen = new List<XHud_LayoutSpawner_Item>();
        public List<XHud_LayoutSpawner_Item> SpawnItemList_World = new List<XHud_LayoutSpawner_Item>();

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
        public UnityAction<XHudSpace> act_on_spawn;
        /// <summary>
        /// 动作 - 回收元素
        /// </summary>
        public UnityAction<XHudSpace> act_on_despawn;
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
                            Spawn(XHudSpace.屏幕空间);
                        }
                        if (ControlWorld)
                        {
                            Spawn(XHudSpace.世界空间);
                        }
                    }

                    if (Input.GetKeyDown(Key_Recycle))
                    {
                        if (!UseManullyKey)
                            return;
                        if (ControlScreen)
                        {
                            Despawn(XHudSpace.屏幕空间);
                        }
                        if (ControlWorld)
                        {
                            Despawn(XHudSpace.世界空间);
                        }
                    }
                }
        }

        #region 调用方法
        /// <summary>
        /// 根据空间类型生成元素
        /// </summary>
        /// <param tweenName="space"></param>
        public void Spawn(XHudSpace space)
        {
            if (!UseManullyKey)
                return;

            switch (space)
            {
                case XHudSpace.屏幕空间:
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
                case XHudSpace.世界空间:
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
        public void Despawn(XHudSpace space)
        {
            switch (space)
            {
                case XHudSpace.屏幕空间:
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
                case XHudSpace.世界空间:
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
                XHud_LayoutSpawner_Item item = SpawnItemList_Screen[i];
                if (!item.InMotion && !item.Spawned)
                    StartCoroutine(hsp_Create(item, XHudSpace.屏幕空间));
            }
            if (act_on_spawn != null)
                act_on_spawn(XHudSpace.屏幕空间);
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
                XHud_LayoutSpawner_Item item = SpawnItemList_Screen[i];
                if (!item.InMotion && item.Spawned)
                    StartCoroutine(hsp_Recycle(item));
            }
            if (act_on_despawn != null)
                act_on_despawn(XHudSpace.屏幕空间);
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
                XHud_LayoutSpawner_Item item = SpawnItemList_World[i];
                if (!item.InMotion && !item.Spawned)
                    StartCoroutine(hsp_Create(item, XHudSpace.世界空间));
            }
            if (act_on_spawn != null)
                act_on_spawn(XHudSpace.世界空间);
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
                XHud_LayoutSpawner_Item item = SpawnItemList_World[i];
                if (!item.InMotion && item.Spawned)
                    StartCoroutine(hsp_Recycle(item));
            }
            if (act_on_despawn != null)
                act_on_despawn(XHudSpace.世界空间);
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
            StartCoroutine(hsp_OrderDelay_Create(XHudSpace.屏幕空间));
            if (act_on_spawn != null)
                act_on_spawn(XHudSpace.屏幕空间);
        }

        /// <summary>
        /// 回收Hud元素到池中 - 屏幕级别 - 顺序回收
        /// </summary>
        private void hsp_Screen_OrderDelay_Despawn()
        {
            if (CheckElementLibrary_IsEmpty())
                return;
            StartCoroutine(hsp_OrderDelay_Recycle(XHudSpace.屏幕空间));
            if (act_on_despawn != null)
                act_on_despawn(XHudSpace.屏幕空间);
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
            StartCoroutine(hsp_OrderDelay_Create(XHudSpace.世界空间));
            if (act_on_spawn != null)
                act_on_spawn(XHudSpace.世界空间);
        }

        /// <summary>
        /// 回收Hud元素到池中 - 世界级别 - 顺序回收
        /// </summary>
        private void hsp_World_OrderDelay_Despawn()
        {
            if (CheckElementLibrary_IsEmpty())
                return;
            StartCoroutine(hsp_OrderDelay_Recycle(XHudSpace.世界空间));
            if (act_on_despawn != null)
                act_on_despawn(XHudSpace.世界空间);
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
        private IEnumerator hsp_Create(XHud_LayoutSpawner_Item item, XHudSpace space)
        {
            //---延迟创建
            yield return new WaitForSeconds(item.Delay_Spawn);
            //---生成元素
            switch (space)
            {
                case XHudSpace.屏幕空间:

                    item.SpawnedElementNode = XHud_Manager.Instance.hm_ScreenElement_Create(
                        LibName,
                        item.Indicator,
                        item.SpawnName,
                        item.UseSpawnerMotion == "自身动效" ? item.CreateArgs : CreateArgs,
                        item.AutoIn == "自动 In" ? true : false,
                        item.Position + item.Offset,
                        item.Scale,
                        item.Size,
                        false,
                        null,
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
                       });
                    break;
                case XHudSpace.世界空间:

                    item.SpawnedElementNode = XHud_Manager.Instance.hm_WorldElement_Create(
                        LibName,
                        item.Indicator,
                        item.SpawnName,
                        item.UseSpawnerMotion == "自身动效" ? item.CreateArgs : CreateArgs,
                        item.AutoIn == "自动 In" ? true : false,
                        item.Size,
                        item.Position,
                        item.Euler,
                        item.Scale,
                        item.Offset,
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
                        });
                    break;
            }
        }

        /// <summary>
        /// 回收
        /// </summary>
        /// <param tweenName="item"></param>
        /// <returns></returns>
        private IEnumerator hsp_Recycle(XHud_LayoutSpawner_Item item)
        {
            //---延迟回收
            yield return new WaitForSeconds(item.Delay_Despawn);
            //---回收元素
            XHud_Manager.Instance.hm_HudElement_RecycleAt(
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
        private IEnumerator hsp_OrderDelay_Create(XHudSpace space)
        {
            //---生成元素
            switch (space)
            {
                case XHudSpace.屏幕空间:
                    for (int i = 0; i < SpawnItemList_Screen.Count; i++)
                    {
                        XHud_LayoutSpawner_Item item = SpawnItemList_Screen[i];
                        if (!item.InMotion && !item.Spawned)
                        {
                            //---延迟创建
                            yield return new WaitForSeconds(item.Delay_Spawn + DelayOrder_Spawn_Screen);
                            item.SpawnedElementNode = XHud_Manager.Instance.hm_ScreenElement_Create(
                                LibName,
                                item.Indicator,
                                item.SpawnName,
                                item.UseSpawnerMotion == "自身动效" ? item.CreateArgs : CreateArgs,
                                item.AutoIn == "自动 In" ? true : false,
                                item.Position + item.Offset,
                                item.Scale,
                                item.Size,
                                false,
                                null,
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
                                });
                        }
                    }
                    break;
                case XHudSpace.世界空间:
                    for (int i = 0; i < SpawnItemList_World.Count; i++)
                    {
                        XHud_LayoutSpawner_Item item = SpawnItemList_World[i];
                        if (!item.InMotion && !item.Spawned)
                        {
                            //---延迟创建
                            yield return new WaitForSeconds(item.Delay_Spawn + DelayOrder_Spawn_World);
                            item.SpawnedElementNode = XHud_Manager.Instance.hm_WorldElement_Create(
                                LibName,
                                item.Indicator,
                                item.SpawnName,
                                item.UseSpawnerMotion == "自身动效" ? item.CreateArgs : CreateArgs,
                                item.AutoIn == "自动 In" ? true : false,
                                item.Size,
                                item.Position,
                                item.Euler,
                                item.Scale,
                                item.Offset,
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
                                });
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
        private IEnumerator hsp_OrderDelay_Recycle(XHudSpace space)
        {
            switch (space)
            {
                case XHudSpace.屏幕空间:
                    for (int i = 0; i < SpawnItemList_Screen.Count; i++)
                    {
                        XHud_LayoutSpawner_Item item = SpawnItemList_Screen[i];

                        if (!item.InMotion && item.Spawned)
                        {
                            //---延迟回收
                            yield return new WaitForSeconds(item.Delay_Despawn + DelayOrder_Despawn_Screen);

                            //---回收元素
                            XHud_Manager.Instance.hm_HudElement_RecycleAt(
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
                case XHudSpace.世界空间:
                    for (int i = 0; i < SpawnItemList_World.Count; i++)
                    {
                        XHud_LayoutSpawner_Item item = SpawnItemList_World[i];

                        if (!item.InMotion && item.Spawned)
                        {
                            //---延迟回收
                            yield return new WaitForSeconds(item.Delay_Despawn + DelayOrder_Despawn_World);

                            //---回收元素
                            XHud_Manager.Instance.hm_HudElement_RecycleAt(
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

            if (XHud_Manager.Instance.Hud_ElementLibrarys == null)
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