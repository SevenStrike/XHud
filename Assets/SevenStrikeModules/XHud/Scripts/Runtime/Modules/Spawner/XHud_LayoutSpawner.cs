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
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XTween;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// 布局生成器参数配置类
    /// </summary>
    [System.Serializable]
    public class XHud_LayoutSpawner_Args
    {
        /// <summary>元素库名称</summary>
        public string libname;
        /// <summary>待生成的元素项列表</summary>
        public List<XHud_LayoutSpawner_Item> SpawnItemList = new List<XHud_LayoutSpawner_Item>();
        /// <summary>生成器标识符</summary>
        public string SpawnerIndicator;

        /// <summary>生成动效参数</summary>
        public Motion_Creator CreateArgs;
        /// <summary>生成参数名称</summary>
        public string CreateParamName;
        /// <summary>CRC校验库名称</summary>
        public string Crc_Lib_Name;

        /// <summary>回收动效参数</summary>
        public Motion_Recycler RecycleArgs;
        /// <summary>回收参数名称</summary>
        public string RecycleParamName;
        /// <summary>回收库名称</summary>
        public string Rec_Lib_Name;

        /// <summary>生成快捷键</summary>
        public KeyCode Key_Create = KeyCode.F7;
        /// <summary>回收快捷键</summary>
        public KeyCode Key_Recycle = KeyCode.F8;

        /// <summary>是否使用手动按键控制</summary>
        public bool UseManullyKey;
        /// <summary>是否正在生成中</summary>
        public bool IsSpawning;

        /// <summary>生成动效-折叠移动效果</summary>
        public bool create_fold_move;
        /// <summary>生成动效-折叠旋转效果</summary>
        public bool create_fold_rotate;
        /// <summary>生成动效-折叠透明度效果</summary>
        public bool create_fold_alpha;
        /// <summary>回收动效-折叠移动效果</summary>
        public bool recycle_fold_move;
        /// <summary>回收动效-折叠旋转效果</summary>
        public bool recycle_fold_rotate;
        /// <summary>回收动效-折叠透明度效果</summary>
        public bool recycle_fold_alpha;
    }

    /// <summary>
    /// 布局生成器元素项配置类
    /// </summary>
    [System.Serializable]
    public class XHud_LayoutSpawner_Item
    {
        /// <summary>生成的元素名称</summary>
        public string SpawnName;
        /// <summary>标识名称</summary>
        public string Indicator;
        /// <summary>ID</summary>
        public string ID;
        /// <summary>生成后是否清空事件</summary>
        public bool ClearEvents;
        /// <summary>生成后是否清空委托</summary>
        public bool ClearActions;
        /// <summary>生成延迟时间（秒）</summary>
        public float Delay_Spawn;
        /// <summary>回收延迟时间（秒）</summary>
        public float Delay_Despawn;
        /// <summary>生成的世界坐标位置</summary>
        public Vector3 Position = Vector3.one;
        /// <summary>生成的欧拉旋转角度</summary>
        public Vector3 Euler = Vector3.zero;
        /// <summary>生成的偏移量</summary>
        public Vector3 Offset = Vector3.zero;
        /// <summary>生成的缩放比例</summary>
        public Vector3 Scale = Vector3.one;
        /// <summary>生成的RectTransform尺寸</summary>
        public Vector2 Size;
        /// <summary>生成的轴心点</summary>
        public Vector2 Pivot;
        /// <summary>生成的锚点最小值</summary>
        public Vector2 Anchor_Min;
        /// <summary>生成的锚点最大值</summary>
        public Vector2 Anchor_Max;
        /// <summary>生成的元素是否自动激活入场动效（"自动 In" 表示自动激活）</summary>
        public string AutoIn = "自动 In";
        /// <summary>是否正在播放动效中</summary>
        public bool InMotion;
        /// <summary>是否已生成</summary>
        public bool Spawned;
        /// <summary>生成的元素节点引用</summary>
        public XHudElementNode SpawnedElementNode;
        /// <summary>使用的动效源（"自身动效" 表示使用元素自身动效，否则使用生成器全局动效）</summary>
        public string UseSpawnerMotion = "列表项动效";
        /// <summary>生成动效参数（当使用自身动效时生效）</summary>
        public Motion_Creator CreateArgs = new Motion_Creator();
        /// <summary>回收动效参数（当使用自身动效时生效）</summary>
        public Motion_Recycler RecycleArgs = new Motion_Recycler();
        /// <summary>动效播放进度（0-1）</summary>
        public float MotionPercentage;
        /// <summary>编辑器折叠状态</summary>
        public bool isFold = true;
        /// <summary>是否启用该项</summary>
        public bool isEnabled = true;

        /// <summary>元素入场开始时的回调</summary>
        public UnityAction<XHud_Module_Element> act_on_element_in_start;
        /// <summary>元素入场结束时的回调</summary>
        public UnityAction<XHud_Module_Element> act_on_element_in_end;
        /// <summary>元素出场开始时的回调</summary>
        public UnityAction<XHud_Module_Element> act_on_element_out_start;
        /// <summary>元素出场结束时的回调</summary>
        public UnityAction<XHud_Module_Element> act_on_element_out_end;
    }

    /// <summary>
    /// XHud布局生成器组件
    /// 负责管理UI元素的批量生成、回收和动效控制
    /// </summary>
    public class XHud_LayoutSpawner : MonoBehaviour
    {
        #region 字段
        /// <summary>
        ///关联的元素库名称
        ///</summary>
        [SerializeField] public string LibName;
        /// <summary>
        ///编辑器标记-屏幕空间布局是否已加载
        ///</summary>
        [SerializeField] public bool IsLoadedLayout_Screen;
        /// <summary>
        ///编辑器标记-世界空间布局是否已加载
        ///</summary>
        [SerializeField] public bool IsLoadedLayout_World;
        /// <summary>
        ///屏幕空间待生成的元素项列表
        ///</summary>
        [SerializeField] public List<XHud_LayoutSpawner_Item> SpawnItemList_Screen = new List<XHud_LayoutSpawner_Item>();
        /// <summary>
        ///世界空间待生成的元素项列表
        ///</summary>
        [SerializeField] public List<XHud_LayoutSpawner_Item> SpawnItemList_World = new List<XHud_LayoutSpawner_Item>();
        /// <summary>
        ///生成器标识符
        ///</summary>
        [SerializeField] public string SpawnerIndicator;
        /// <summary>
        ///全局生成动效参数（当元素未使用自身动效时生效）
        ///</summary>
        [SerializeField]
        public Motion_Creator CreateArgs = new Motion_Creator
        {
            anchor = XHudAnchor.中心,
            Movement = new MotionNode_Movement()
            {
                Movement = HudMotion_Movement.S_从下至上,
                Distance = 100,
                Duration = 1,
                Delay = 0,
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                CurveName = "",
                Ease = EaseMode.InOutCubic,
            },
            Rotation = new MotionNode_Rotation()
            {
                Rotation = HudMotion_Rotation.A_无旋转,
                Degree = 0,
                Duration = 1,
                Delay = 0,
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                CurveName = "",
                Ease = EaseMode.InOutCubic,
            },
            Alpha = new MotionNode_Alpha()
            {
                Duration = 1,
                Delay = 0,
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                CurveName = "",
                Ease = EaseMode.InOutCubic,
            }
        };
        /// <summary>
        ///生成参数名称
        ///</summary>
        [SerializeField] public string CreateParamName;
        /// <summary>
        ///CRC校验库名称
        ///</summary>
        [SerializeField] public string Crc_Lib_Name;
        /// <summary>
        ///全局回收动效参数（当元素未使用自身动效时生效）
        ///</summary>
        [SerializeField]
        public Motion_Recycler RecycleArgs = new Motion_Recycler
        {
            MotionAnimateEndState = MotionAnimateEndState.以_透明度为准,
            Movement = new MotionNode_Movement()
            {
                Movement = HudMotion_Movement.D_从上至下,
                Distance = 100,
                Duration = 1,
                Delay = 0,
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                CurveName = "",
                Ease = EaseMode.InOutCubic
            },
            Rotation = new MotionNode_Rotation()
            {
                Rotation = HudMotion_Rotation.A_无旋转,
                Degree = 0,
                Duration = 1,
                Delay = 0,
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                CurveName = "",
                Ease = EaseMode.InOutCubic
            },
            Alpha = new MotionNode_Alpha()
            {
                Duration = 1,
                Delay = 0,
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                CurveName = "",
                Ease = EaseMode.InOutCubic
            }
        };
        /// <summary>
        ///回收参数名称
        ///</summary>
        [SerializeField] public string RecycleParamName;
        /// <summary>
        ///回收库名称
        ///</summary>
        [SerializeField] public string Rec_Lib_Name;
        /// <summary>
        ///主功能键（组合键）
        ///</summary>
        [SerializeField] public SpawnFunctionKey SpawnFunctionKey_Primary = SpawnFunctionKey.Shift;
        /// <summary>
        ///辅助功能键（组合键）
        ///</summary>
        [SerializeField] public SpawnFunctionKey SpawnFunctionKey_Secondary = SpawnFunctionKey.None;
        /// <summary>
        ///生成快捷键
        ///</summary>
        [SerializeField] public KeyCode Key_Create = KeyCode.G;
        /// <summary>
        ///回收快捷键
        ///</summary>
        [SerializeField] public KeyCode Key_Recycle = KeyCode.R;
        /// <summary>
        ///是否使用手动按键控制
        ///</summary>
        [SerializeField] public bool UseManullyKey;
        /// <summary>
        ///是否启用调试日志
        ///</summary>
        [SerializeField] public bool UseDebug;
        /// <summary>
        ///是否使用实例化模式（直接实例化而非从库中获取）
        ///</summary>
        [SerializeField] public bool UseInstantiateMode = true;
        /// <summary>
        ///是否启用屏幕空间的按键控制
        ///</summary>
        [SerializeField] public bool KeyControl_Screen = true;
        /// <summary>
        ///是否启用世界空间的按键控制
        ///</summary>
        [SerializeField] public bool KeyControl_World = true;
        /// <summary>
        ///编辑器-屏幕空间折叠状态
        ///</summary>
        [SerializeField] public bool FoldScreen = true;
        /// <summary>
        ///编辑器-世界空间折叠状态
        ///</summary>
        [SerializeField] public bool FoldWorld = true;
        /// <summary>
        ///编辑器预览模式
        ///</summary>
        [SerializeField] public bool IsPreviewing = false;
        /// <summary>
        ///所有动效播放状态（是否有任何元素正在播放动效）
        ///</summary>
        [SerializeField] private bool spawnerisRunning = false;
        /// <summary>
        ///动效播放状态属性（当状态改变时触发回调）
        ///</summary>
        [SerializeField]
        public bool SpawnerisRunning
        {
            get
            {
                return spawnerisRunning;
            }
            set
            {
                if (value != spawnerisRunning)
                {
                    spawnerisRunning = value;
                    if (act_on_motion_state_changed != null)
                        act_on_motion_state_changed(value);
                }
            }
        }
        /// <summary>
        ///生成动效-折叠移动效果
        ///</summary>
        [SerializeField] public bool create_fold_move;
        /// <summary>
        ///生成动效-折叠旋转效果
        ///</summary>
        [SerializeField] public bool create_fold_rotate;
        /// <summary>
        ///生成动效-折叠透明度效果
        ///</summary>
        [SerializeField] public bool create_fold_alpha;
        /// <summary>
        ///回收动效-折叠移动效果
        ///</summary>
        [SerializeField] public bool recycle_fold_move;
        /// <summary>
        ///回收动效-折叠旋转效果
        ///</summary>
        [SerializeField] public bool recycle_fold_rotate;
        /// <summary>
        ///回收动效-折叠透明度效果
        ///</summary>
        [SerializeField] public bool recycle_fold_alpha;
        #endregion

        #region 委托
        /// <summary>
        ///生成动作回调（参数：目标空间类型）
        ///</summary>
        public UnityAction<XHudSpace> act_on_spawn;
        /// <summary>
        ///生成动作回调（参数：目标空间类型）
        ///</summary>
        public UnityAction<XHudSpace, XHudElementNode, string> act_on_spawn_with_id;
        /// <summary>
        ///回收动作回调（参数：目标空间类型）
        ///</summary>
        public UnityAction<XHudSpace> act_on_despawn;
        /// <summary>
        ///回收动作回调（参数：目标空间类型）
        ///</summary>
        public UnityAction<XHudSpace, XHud_Module_Element, string> act_on_despawn_with_id;
        /// <summary>
        ///元素动效状态改变回调（参数：是否正在播放动效）
        ///</summary>
        public UnityAction<bool> act_on_motion_state_changed;
        #endregion

        /// <summary>
        ///存储所有正在执行的生成协程
        ///</summary>
        private List<Coroutine> SpawnCoroutines = new List<Coroutine>();
        /// <summary>
        ///存储所有正在执行的回收协程
        ///</summary>
        private List<Coroutine> DespawnCoroutines = new List<Coroutine>();

        private void Awake()
        {

        }

        private void Start()
        {
            //SyncLayoutItemsMotionArgs();
        }

        private void Update()
        {
            ProtectedActionState();
            KeyControl();
        }

        #region 清理 & 停止 协程列表
        /// <summary>
        /// 停止所有正在执行的生成协程
        /// </summary>
        private void StopSpawnCoroutines()
        {
            for (int i = 0; i < SpawnCoroutines.Count; i++)
            {
                Coroutine cor = SpawnCoroutines[i];
                if (cor != null)
                    StopCoroutine(cor);

                SpawnCoroutines[i] = null;
            }

            SpawnCoroutines.Clear();
        }
        /// <summary>
        /// 停止所有正在执行的回收协程
        /// </summary>
        private void StopDespawnCoroutines()
        {
            for (int i = 0; i < DespawnCoroutines.Count; i++)
            {
                Coroutine cor = DespawnCoroutines[i];
                if (cor != null)
                    StopCoroutine(cor);

                DespawnCoroutines[i] = null;
            }

            DespawnCoroutines.Clear();
        }
        #endregion

        #region 生成逻辑
        /// <summary>
        /// 根据空间类型批量生成元素
        /// </summary>
        /// <param name="space">目标空间类型（屏幕空间/世界空间）</param>
        public void Spawn(XHudSpace space)
        {
            StopDespawnCoroutines();

            #region 清理回收元素项

            for (int i = 0; i < (space == XHudSpace.屏幕空间 ? SpawnItemList_Screen.Count : SpawnItemList_World.Count); i++)
            {
                XHud_LayoutSpawner_Item item = (space == XHudSpace.屏幕空间 ? SpawnItemList_Screen[i] : SpawnItemList_World[i]);

                if (!item.Spawned && item.SpawnedElementNode.Element == null)
                    continue;

                if (item.Spawned)
                {
                    item.MotionPercentage = 0;
                    item.Spawned = false;
                }

                if (item.SpawnedElementNode.Element != null)
                {
                    element_Despawn(item, space);
                }
            }

            #endregion

            // 检查判断元素库不是空的
            if (CheckElementLibrary_IsEmpty())
                return;

            #region 判断目标类型布局列表是否是空的

            if (space == XHudSpace.屏幕空间)
            {
                // 如果“屏幕”布局列表是空的直接返回
                if (ScreenLayoutIsEmpty())
                    return;
            }
            else
            {
                // 如果“世界”布局列表是空的直接返回
                if (WroldLayoutIsEmpty())
                    return;
            }

            #endregion

            #region 启动多个协程执行列表中元素生成逻辑

            for (int i = 0; i < (space == XHudSpace.屏幕空间 ? SpawnItemList_Screen.Count : SpawnItemList_World.Count); i++)
            {
                XHud_LayoutSpawner_Item item = (space == XHudSpace.屏幕空间 ? SpawnItemList_Screen[i] : SpawnItemList_World[i]);

                if (!item.isEnabled)
                    continue;

                // 如果已经生成则忽略
                if (item.Spawned)
                    continue;

                // 启动生成后将此协程加入生成协程列表便于中断控制
                SpawnCoroutines.Add(StartCoroutine(cor_Spawn(item, space)));
            }

            #endregion

            CheckCoroutinesStatistic(true);

            // 执行状态回调 - 根据布局已生成所有元素
            if (act_on_spawn != null)
                act_on_spawn(space);
        }
        /// <summary>
        /// 生成协程逻辑（处理延迟生成）
        /// </summary>
        /// <param name="item">待生成的元素项</param>
        /// <param name="space">目标空间类型</param>
        /// <returns>协程迭代器</returns>
        private IEnumerator cor_Spawn(XHud_LayoutSpawner_Item item, XHudSpace space)
        {
            // 等待延时
            yield return new WaitForSeconds(item.Delay_Spawn);
            // 生成元素
            element_Spawn(item, space);
        }
        /// <summary>
        /// 通用生成元素逻辑
        /// </summary>
        /// <param name="item">待生成的元素项</param>
        /// <param name="space">目标空间类型</param>
        private void element_Spawn(XHud_LayoutSpawner_Item item, XHudSpace space)
        {
            #region （实例化 ”SpawnedElementNode“ 类）             

            XHud_Module_Element e = null;

            // 该方法是直接实例化元素（非从元素库中取元素）
            if (UseInstantiateMode)
            {
                e = XHud_Manager.Instance.hm_ElementLibrary_GetTargetLibrary(LibName).ElementsLibrary_GetTargetElement(item.SpawnName);
            }

            if (space == XHudSpace.屏幕空间)
            {
                if (UseInstantiateMode) // 该方法是直接实例化元素
                {
                    item.SpawnedElementNode = XHud_Manager.Instance.hm_ScreenElement_Create(Instantiate(e));
                }
                else // 从库中找到匹配的元素则返回实例
                    item.SpawnedElementNode = XHud_Manager.Instance.hm_ScreenElement_Create(LibName, item.SpawnName);
            }
            else
            {
                if (UseInstantiateMode) // 该方法是直接实例化元素
                {
                    item.SpawnedElementNode = XHud_Manager.Instance.hm_WorldElement_Create(Instantiate(e));
                }
                else // 从库中找到匹配的元素则返回实例
                {
                    item.SpawnedElementNode = XHud_Manager.Instance.hm_WorldElement_Create(LibName, item.SpawnName);
                }
            }

            XHudElementNode node = item.SpawnedElementNode;
            #endregion

            // 动效源选择（自身动效 or 全局动效）
            Motion_Creator arg = item.UseSpawnerMotion == "列表项动效" ? item.CreateArgs : item.UseSpawnerMotion == "元素自身动效" ? node.Element.CreateArgs : CreateArgs;

            // 如果无法再库中找到匹配的元素则返回一个空的实例
            if (node == null)
            {
                if (UseDebug)
                {
                    string hex_col = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

                    XGUI_Utilitys.Console("XHud - 布局元素生成器通知", $"未在元素库： <b><color={hex_col}>{LibName} </color></b>中找到：<b><color={hex_col}> {item.SpawnName} </color></b>元素！请检查目标元素库中是否存在该元素！", XGUIMsgState.通知);
                }
                node = new XHudElementNode();
                item.Spawned = false;
                return;
            }

            // 如果在库中找到匹配的元素则返回实例 - 将列表中对应的生成项设置为“已生成”标记
            item.Spawned = true;

            // 调用生成委托，并传递生成的元素结构以及生成项的ID号
            // 便于附加的脚本对匹配的ID的元素结构中的元素中的控件进行数值绑定
            if (act_on_spawn_with_id != null)
            {
                act_on_spawn_with_id(space, node, item.ID);
            }

            #region 设置元素

            if (space == XHudSpace.屏幕空间)
            {
                // 设置元素的初始状态
                node
                .SetAlpha(0)
                .SetAnchored_Screen(arg.anchor, item.Indicator)
                .SetPosition_Screen(item.Position)
                .SetRotation_Screen(item.Euler)
                .SetOffset(item.Offset)
                .SetSize(item.Size)
                .SetScale(item.Scale)
                .On_In_Start((e) =>
                {
                    // 入场开始时 - 运动状态启用
                    item.InMotion = true;
                    // 入场开始时 - 委托回调
                    if (item.act_on_element_in_start != null)
                    {
                        item.act_on_element_in_start(e);
                    }
                })
                .On_In_Progress((progress) =>
                {
                    // 入场过程中 - 进度条同步
                    item.MotionPercentage = progress;
                })
                .On_In_End((e) =>
                {
                    // 入场结束后 - 运动状态禁用
                    item.InMotion = false;
                    // 入场结束后 - 委托回调
                    if (item.act_on_element_in_end != null)
                    {
                        item.act_on_element_in_end(e);
                    }
                })
                .On_Out_Start((e) =>
                {
                    // 出场开始时 - 运动状态启用
                    item.InMotion = true;
                    // 出场开始时 - 委托回调
                    if (item.act_on_element_out_start != null)
                    {
                        item.act_on_element_out_start(e);
                    }
                })
                .On_Out_Progress((progress) =>
                {
                    // 出场过程中 - 进度条同步
                    item.MotionPercentage = progress;
                })
                .On_Out_End((e) =>
                {
                    // 出场结束后 - 运动状态禁用
                    item.InMotion = false;
                    // 出场结束后 - 已生成状态禁用
                    item.Spawned = false;
                    // 出场结束后 - 委托回调
                    if (item.act_on_element_out_end != null)
                    {
                        item.act_on_element_out_end(e);
                    }
                    // 出场结束后 - 清空已生成元素节点实例
                    //element_node = null;
                });
            }
            else
            {
                // 设置元素的初始状态
                node
                .SetAlpha(0)
                .SetAnchored_World(item.Indicator)
                .SetPosition_World(item.Position)
                .SetRotation_World(Quaternion.Euler(item.Euler))
                .SetOffset(item.Offset)
                .SetSize(item.Size)
                .SetScale(item.Scale)
                .On_In_Start((e) =>
                {
                    // 入场开始时 - 运动状态启用
                    item.InMotion = true;
                    // 入场开始时 - 委托回调
                    if (item.act_on_element_in_start != null)
                    {
                        item.act_on_element_in_start(e);
                    }
                })
                .On_In_Progress((progress) =>
                {
                    // 入场过程中 - 进度条同步
                    item.MotionPercentage = progress;
                })
                .On_In_End((e) =>
                {
                    // 入场结束后 - 运动状态禁用
                    item.InMotion = false;
                    // 入场结束后 - 委托回调
                    if (item.act_on_element_in_end != null)
                    {
                        item.act_on_element_in_end(e);
                    }
                })
                .On_Out_Start((e) =>
                {
                    // 出场开始时 - 运动状态启用
                    item.InMotion = true;
                    // 出场开始时 - 委托回调
                    if (item.act_on_element_out_start != null)
                    {
                        item.act_on_element_out_start(e);
                    }
                })
                .On_Out_Progress((progress) =>
                {
                    // 出场过程中 - 进度条同步
                    item.MotionPercentage = 1 - progress;
                })
                .On_Out_End((e) =>
                {
                    // 出场结束后 - 运动状态禁用
                    item.InMotion = false;
                    // 出场结束后 - 已生成状态禁用
                    item.Spawned = false;
                    // 出场结束后 - 委托回调
                    if (item.act_on_element_out_end != null)
                    {
                        item.act_on_element_out_end(e);
                    }
                    // 出场结束后 - 清空已生成元素节点实例
                    //element_node = null;
                });
            }

            #endregion

            // 根据自动播放标记激活元素入场逻辑
            if (item.AutoIn == "自动 In")
            {
                node.Element_In(arg);
            }
        }
        #endregion

        #region 回收逻辑
        /// <summary>
        /// 根据空间类型批量回收元素
        /// </summary>
        /// <param name="space">目标空间类型（屏幕空间/世界空间）</param>
        public void Despawn(XHudSpace space)
        {
            StopSpawnCoroutines();

            // 检查判断元素库不是空的
            if (CheckElementLibrary_IsEmpty())
                return;

            #region 判断目标类型布局列表是否是空的

            if (space == XHudSpace.屏幕空间)
            {
                // 如果“屏幕”布局列表是空的直接返回
                if (ScreenLayoutIsEmpty())
                    return;
            }
            else
            {
                // 如果“世界”布局列表是空的直接返回
                if (WroldLayoutIsEmpty())
                    return;
            }

            #endregion

            #region 启动多个协程执行列表中元素回收逻辑

            for (int i = 0; i < (space == XHudSpace.屏幕空间 ? SpawnItemList_Screen.Count : SpawnItemList_World.Count); i++)
            {
                XHud_LayoutSpawner_Item item = (space == XHudSpace.屏幕空间 ? SpawnItemList_Screen[i] : SpawnItemList_World[i]);

                if (!item.isEnabled)
                    continue;

                // 启动回收协程列表
                DespawnCoroutines.Add(StartCoroutine(cor_Despawn(item, space)));
            }

            #endregion

            // 执行状态回调 - 根据布局已回收所有元素
            if (act_on_despawn != null)
                act_on_despawn(space);
        }
        /// <summary>
        /// 回收协程逻辑（处理延迟回收）
        /// </summary>
        /// <param name="item">待回收的元素项</param>
        /// <returns>协程迭代器</returns>
        private IEnumerator cor_Despawn(XHud_LayoutSpawner_Item item, XHudSpace space)
        {
            // 等待延时
            yield return new WaitForSeconds(item.Delay_Despawn);
            // 回收元素
            element_Despawn(item, space);
        }
        /// <summary>
        /// 通用回收元素逻辑
        /// </summary>
        /// <param name="item">待回收的元素项</param>
        private void element_Despawn(XHud_LayoutSpawner_Item item, XHudSpace space)
        {
            XHudElementNode node = item.SpawnedElementNode;

            if (node != null)
            {
                // 获取列表项中的已生成的元素
                XHud_Module_Element ele = node.Element;

                item.Spawned = false;

                // 如果已生成的元素不为空
                if (ele != null)
                {
                    // 有管理器负责的回收逻辑开始执行（根据动效源选择使用自身动效或全局动效）
                    XHud_Manager.Instance.hm_HudElement_RecycleAt(node.Element, item.UseSpawnerMotion == "列表项动效" ? item.RecycleArgs : item.UseSpawnerMotion == "元素自身动效" ? ele.RecycleArgs : RecycleArgs, action_out_end: (ele) =>
                    {
                        // 调用回收委托，并传递回收的元素结构以及回收项的ID号
                        // 便于附加的脚本对匹配的ID的元素结构中的元素中的控件进行数值绑定
                        if (act_on_despawn_with_id != null)
                        {
                            act_on_despawn_with_id(space, ele, item.ID);
                        }
                    });
                    node.Element = null;
                }
            }
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 按键控制逻辑（检测快捷键组合并触发生成/回收）
        /// </summary>
        private void KeyControl()
        {
            if (FunctionKey_Primary_Detect() && FunctionKey_Secondary_Detect())
            {
                if (Input.GetKeyDown(Key_Create))
                {
                    if (!UseManullyKey)
                        return;
                    if (KeyControl_Screen)
                    {
                        Spawn(XHudSpace.屏幕空间);
                    }
                    else if (KeyControl_World)
                    {
                        Spawn(XHudSpace.世界空间);
                    }
                }

                if (Input.GetKeyDown(Key_Recycle))
                {
                    if (!UseManullyKey)
                        return;
                    if (KeyControl_Screen)
                    {
                        Despawn(XHudSpace.屏幕空间);
                    }
                    else if (KeyControl_World)
                    {
                        Despawn(XHudSpace.世界空间);
                    }
                }
            }
        }
        /// <summary>
        /// 输出协程统计信息（用于调试）
        /// </summary>
        /// <param name="Enabled">是否启用调试输出</param>
        private void CheckCoroutinesStatistic(bool Enabled)
        {
            if (!Enabled)
                return;

            if (UseDebug)
            {
                string hex_col = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);
                string msg = $"coroutines_Spawn 列表统计：{SpawnerIndicator}  目前存在   <b><color={hex_col}>{SpawnCoroutines.Count}</color></b>   个";
                XGUI_Utilitys.Console("XHud - 布局元素生成器通知", msg, XGUIMsgState.警告);
            }
        }
        /// <summary>
        /// 检测主功能键是否按下
        /// </summary>
        /// <returns>主功能键是否处于激活状态</returns>
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
        /// 检测辅助功能键是否按下
        /// </summary>
        /// <returns>辅助功能键是否处于激活状态</returns>
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

        /* SyncLayoutItemsMotionArgs 解释：
         * -----------------
         * 因为如果被布局元素生成器收集之后，更改了元素的动效参数，
         * 则会通过此步骤进行应用启动前的更新以此确保生成的动效的同源同步        
         */

        /// <summary>
        /// 从元素库同步动效参数到待生成的元素列表项
        /// 确保在启动前将元素库中配置的动效参数同步到布局生成器中
        /// </summary>
        public void SyncLayoutItemsMotionArgs()
        {
            if ((SpawnItemList_Screen == null && SpawnItemList_Screen.Count <= 0) || (SpawnItemList_World == null && SpawnItemList_World.Count <= 0))
                return;

            // 匹配元素库
            XHud_Library_Element ele_lib = Application.isPlaying ? XHud_Manager.Instance.hm_ElementLibrary_GetTargetLibrary(LibName) : XHud_Dashboard.HudManagerGet().hm_ElementLibrary_GetTargetLibrary(LibName);

            if (ele_lib == null)
                return;

            for (int i = 0; i < ele_lib.ElementLibrary.Count; i++)
            {
                XHud_LibraryArg_Element_Item lib_item = ele_lib.ElementLibrary[i];
                if (lib_item == null)
                    continue;

                if (lib_item.Target == null)
                    continue;

                // 屏幕空间生成元素校验
                for (int s = 0; s < SpawnItemList_Screen.Count; s++)
                {
                    XHud_LayoutSpawner_Item spawn_item = SpawnItemList_Screen[s];
                    if (spawn_item.SpawnName == lib_item.Name)
                    {
                        // 这里明确先记录后恢复的原则
                        // 否则会导致布局生成器生成的元素的锚点被元素库的元素自身携带的锚点覆盖
                        // 这样你生成出来的元素是在元素自身携带的生成动效锚点下，而不是布局记录的锚点下
                        XHudAnchor anchor = spawn_item.CreateArgs.anchor;
                        spawn_item.CreateArgs = lib_item.Target.CreateArgs.Clone();
                        spawn_item.CreateArgs.anchor = anchor;

                        spawn_item.RecycleArgs = lib_item.Target.RecycleArgs.Clone();
                    }
                }

                // 世界空间生成元素校验
                for (int s = 0; s < SpawnItemList_World.Count; s++)
                {
                    XHud_LayoutSpawner_Item spawn_item = SpawnItemList_World[s];
                    if (spawn_item.SpawnName == lib_item.Name)
                    {
                        spawn_item.CreateArgs = lib_item.Target.CreateArgs.Clone();
                        spawn_item.RecycleArgs = lib_item.Target.RecycleArgs.Clone();
                    }
                }
            }
        }
        /// <summary>
        /// 检查元素库是否为空
        /// </summary>
        /// <returns>如果元素库为空返回true，否则返回false</returns>
        private bool CheckElementLibrary_IsEmpty()
        {
            bool isEmpty = false;

            if (XHud_Manager.Instance.Hud_ElementLibrarys == null)
                isEmpty = true;

            return isEmpty;
        }
        /// <summary>
        /// 检测所有元素是否正在播放动效，并更新SpawnerisRunning状态
        /// </summary>
        /// <returns>是否有任何元素正在播放动效</returns>
        private bool ProtectedActionState()
        {
            bool hasScreenMotion = false;
            foreach (var item in SpawnItemList_Screen)
            {
                if (item.InMotion)
                {
                    hasScreenMotion = true;
                    break;
                }
            }

            bool hasWorldMotion = false;
            foreach (var item in SpawnItemList_World)
            {
                if (item.InMotion)
                {
                    hasWorldMotion = true;
                    break;
                }
            }

            spawnerisRunning = hasScreenMotion || hasWorldMotion;
            return spawnerisRunning;
        }
        /// <summary>
        /// 检查世界空间布局列表是否为空
        /// </summary>
        /// <returns>如果世界空间布局列表为空返回true，否则返回false</returns>
        private bool WroldLayoutIsEmpty()
        {
            if (SpawnItemList_World.Count <= 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 检查屏幕空间布局列表是否为空
        /// </summary>
        /// <returns>如果屏幕空间布局列表为空返回true，否则返回false</returns>
        private bool ScreenLayoutIsEmpty()
        {
            if (SpawnItemList_Screen.Count <= 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 检查指定空间的元素是否全部生成完毕
        /// </summary>
        /// <param name="space">目标空间类型（屏幕空间/世界空间）</param>
        /// <returns>如果所有元素都已生成返回true，否则返回false</returns>
        public bool IsAllSpawned(XHudSpace space)
        {
            List<XHud_LayoutSpawner_Item> targetList = space == XHudSpace.屏幕空间 ? SpawnItemList_Screen : SpawnItemList_World;

            if (targetList == null || targetList.Count == 0)
                return false;

            foreach (var item in targetList)
            {
                if (item.isEnabled && !item.Spawned)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 检查指定空间的元素是否全部未生成（全部处于回收状态）
        /// </summary>
        /// <param name="space">目标空间类型（屏幕空间/世界空间）</param>
        /// <returns>如果所有元素都未生成返回true，否则返回false</returns>
        public bool IsAllDespawned(XHudSpace space)
        {
            List<XHud_LayoutSpawner_Item> targetList = space == XHudSpace.屏幕空间 ? SpawnItemList_Screen : SpawnItemList_World;

            if (targetList == null || targetList.Count == 0)
                return false;

            foreach (var item in targetList)
            {
                if (item.isEnabled && item.Spawned)
                    return false;
            }

            return true;
        }
        #endregion

        #region 获取指定的元素
        /// <summary>
        /// 根据ID号获取元素项
        /// </summary>
        /// <param name="space"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public XHud_Module_Element GetElement_With_ID(XHudSpace space, string id)
        {
            for (int i = 0; i < (space == XHudSpace.屏幕空间 ? SpawnItemList_Screen.Count : SpawnItemList_World.Count); i++)
            {
                if ((space == XHudSpace.屏幕空间 ? SpawnItemList_Screen[i].ID : SpawnItemList_World[i].ID) == id)
                {
                    if ((space == XHudSpace.屏幕空间 ? SpawnItemList_Screen[i].SpawnedElementNode.Element != null : SpawnItemList_World[i].SpawnedElementNode.Element != null))
                    {
                        return (space == XHudSpace.屏幕空间 ? SpawnItemList_Screen[i].SpawnedElementNode.Element : SpawnItemList_World[i].SpawnedElementNode.Element);
                    }
                    break;
                }
            }

            if (UseDebug)
            {
                string hex_col = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);
                XGUI_Utilitys.Console("XHud - 布局元素生成器通知", $"未找到匹配ID的元素项！请确认生成器的<b><color={hex_col}>  {space.ToString()}  </b></color>列表中存在ID为：<b><color={hex_col}>  {id} </b></color>的元素项！", XGUIMsgState.通知);
            }
            return null;
        }
        /// <summary>
        /// 根据ID号获取元素项
        /// </summary>
        /// <param name="space"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public XHud_Module_Element GetElement_With_ID(string name)
        {
            bool finded = false;

            // 先在“屏幕空间”列表内找
            for (int i = 0; i < SpawnItemList_Screen.Count; i++)
            {
                if (SpawnItemList_Screen[i].ID == name)
                {
                    finded = true;
                    if (SpawnItemList_Screen[i].SpawnedElementNode.Element != null)
                    {
                        return SpawnItemList_Screen[i].SpawnedElementNode.Element;
                    }
                    break;
                }
            }

            // 如果没找到，再在“世界空间”列表内找
            if (!finded)
            {
                for (int i = 0; i < SpawnItemList_World.Count; i++)
                {
                    if (SpawnItemList_World[i].ID == name)
                    {
                        if (SpawnItemList_World[i].SpawnedElementNode.Element != null)
                        {
                            return SpawnItemList_World[i].SpawnedElementNode.Element;
                        }
                        break;
                    }
                }
            }

            if (UseDebug)
            {
                string hex_col = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);
                XGUI_Utilitys.Console("XHud - 布局元素生成器通知", $"未找到匹配ID的元素项！请确认生成器的<b><color={hex_col}>  屏幕空间或是世界空间  </b></color>列表中存在ID为：<b><color={hex_col}>  {name} </b></color>的元素项！", XGUIMsgState.通知);
            }
            return null;
        }
        /// <summary>
        /// 根据名称获取元素项
        /// </summary>
        /// <param name="space"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public XHud_Module_Element GetElement_With_Name(XHudSpace space, string name)
        {
            for (int i = 0; i < (space == XHudSpace.屏幕空间 ? SpawnItemList_Screen.Count : SpawnItemList_World.Count); i++)
            {
                if ((space == XHudSpace.屏幕空间 ? SpawnItemList_Screen[i].SpawnName : SpawnItemList_World[i].SpawnName) == name)
                {
                    if ((space == XHudSpace.屏幕空间 ? SpawnItemList_Screen[i].SpawnedElementNode.Element != null : SpawnItemList_World[i].SpawnedElementNode.Element != null))
                    {
                        return (space == XHudSpace.屏幕空间 ? SpawnItemList_Screen[i].SpawnedElementNode.Element : SpawnItemList_World[i].SpawnedElementNode.Element);
                    }
                    break;
                }
            }

            if (UseDebug)
            {
                string hex_col = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);
                XGUI_Utilitys.Console("XHud - 布局元素生成器通知", $"未找到匹配ID的元素项！请确认生成器的<b><color={hex_col}>  {space.ToString()}  </b></color>列表中存在名称为：<b><color={hex_col}>  {name} </b></color>的元素项！", XGUIMsgState.通知);
            }
            return null;
        }
        /// <summary>
        /// 根据名称号获取元素项
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XHud_Module_Element GetElement_With_Name(string name)
        {
            bool finded = false;

            // 先在“屏幕空间”列表内找
            for (int i = 0; i < SpawnItemList_Screen.Count; i++)
            {
                if (SpawnItemList_Screen[i].SpawnName == name)
                {
                    finded = true;
                    if (SpawnItemList_Screen[i].SpawnedElementNode.Element != null)
                    {
                        return SpawnItemList_Screen[i].SpawnedElementNode.Element;
                    }
                    break;
                }
            }

            // 如果没找到，再在“世界空间”列表内找
            if (!finded)
            {
                for (int i = 0; i < SpawnItemList_World.Count; i++)
                {
                    if (SpawnItemList_World[i].SpawnName == name)
                    {
                        if (SpawnItemList_World[i].SpawnedElementNode.Element != null)
                        {
                            return SpawnItemList_World[i].SpawnedElementNode.Element;
                        }
                        break;
                    }
                }
            }

            if (UseDebug)
            {
                string hex_col = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);
                XGUI_Utilitys.Console("XHud - 布局元素生成器通知", $"未找到匹配 SpawnName 的元素项！请确认生成器的<b><color={hex_col}>  屏幕空间或是世界空间  </b></color>列表中存在SpawnName为：<b><color={hex_col}>  {name} </b></color>的元素项！", XGUIMsgState.通知);
            }
            return null;
        }
        #endregion
    }
}