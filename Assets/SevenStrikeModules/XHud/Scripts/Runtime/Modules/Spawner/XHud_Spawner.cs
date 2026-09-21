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
    using SevenStrikeModules.XTween;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// 生成器功能键枚举
    /// 定义用于触发生成/回收操作的修饰键组合
    /// </summary>
    public enum SpawnFunctionKey
    {
        /// <summary>无修饰键，直接按下触发键即可</summary>
        None = 0,
        /// <summary>Ctrl 键组合</summary>
        Ctrl = 1,
        /// <summary>Alt 键组合</summary>
        Alt = 2,
        /// <summary>Shift 键组合</summary>
        Shift = 3,
    }

    /// <summary>
    /// XHud 元素生成器
    /// 用于在编辑器和运行时动态生成和回收 Hud 元素，支持快捷键操作、视觉预览和世界空间/屏幕空间两种模式
    /// </summary>
    public class XHud_Spawner : MonoBehaviour
    {
        #region 基础配置
        [SerializeField]
        /// <summary>
        /// 元素库名称
        /// 指定从哪个元素库中获取预制体资源
        /// </summary>
        public string LibName;
        [SerializeField]
        /// <summary>
        /// 生成元素名称标识
        /// 用于在元素库中查找对应的预制体名称
        /// </summary>
        public string SpawnName;
        [SerializeField]
        /// <summary>
        /// 生成元素标识符
        /// 用于区分同一预制体的不同实例，可自定义标识名称
        /// </summary>
        public string SpawnIndicator;
        [SerializeField]
        /// <summary>
        /// 生成器自身标识符
        /// 用于标识当前生成器实例，便于管理和调试
        /// </summary>
        public string SpawnerIndicator;
        [SerializeField]
        /// <summary>
        /// 当前生成的元素实例
        /// 保存通过此生成器创建的元素对象引用
        /// </summary>
        public XHud_Module_Element SpawnElement;
        #endregion

        #region 动效参数
        [SerializeField]
        /// <summary>
        /// 元素生成动效参数
        /// 定义元素进入/显示时的动画效果，包括位移、旋转、透明度等
        /// 支持缓动曲线、持续时间、延迟等参数配置
        /// </summary>
        public Motion_Creator CreateArgs = new Motion_Creator
        {
            Alpha = new MotionNode_Alpha
            {
                Duration = 1,           // 透明度动画持续时间（秒）
                Delay = 0,              // 透明度动画延迟时间（秒）
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),  // 透明度动画曲线
                CurveName = "",         // 曲线库中对应的曲线名称
                Ease = EaseMode.InOutCubic  // 缓动模式
            },
            Movement = new MotionNode_Movement
            {
                Movement = HudMotion_Movement.S_从下至上,  // 位移方向类型
                Distance = 100,         // 位移距离（像素）
                Duration = 1,           // 位移动画持续时间（秒）
                Delay = 0,              // 位移动画延迟时间（秒）
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),  // 位移动画曲线
                CurveName = "",         // 曲线库中对应的曲线名称
                Ease = EaseMode.InOutCubic  // 缓动模式
            },
            Rotation = new MotionNode_Rotation
            {
                Rotation = HudMotion_Rotation.A_无旋转,  // 旋转方向类型
                Degree = 0,             // 旋转角度（度）
                Duration = 1,           // 旋转动画持续时间（秒）
                Delay = 0,              // 旋转动画延迟时间（秒）
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),  // 旋转动画曲线
                CurveName = "",         // 曲线库中对应的曲线名称
                Ease = EaseMode.InOutCubic  // 缓动模式
            }
        };
        [SerializeField]
        /// <summary>
        /// 生成动效参数名称
        /// 用于从动效库中加载预设的生成动效配置
        /// </summary>
        public string CreateParamName;
        [SerializeField]
        /// <summary>
        /// 元素回收动效参数
        /// 定义元素退出/隐藏时的动画效果，包括位移、旋转、透明度等
        /// 支持缓动曲线、持续时间、延迟等参数配置
        /// </summary>
        public Motion_Recycler RecycleArgs = new Motion_Recycler
        {
            Alpha = new MotionNode_Alpha
            {
                Duration = 1,           // 透明度动画持续时间（秒）
                Delay = 0,              // 透明度动画延迟时间（秒）
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),  // 透明度动画曲线
                CurveName = "",         // 曲线库中对应的曲线名称
                Ease = EaseMode.InOutCubic  // 缓动模式
            },
            Movement = new MotionNode_Movement
            {
                Movement = HudMotion_Movement.D_从上至下,  // 位移方向类型
                Distance = 100,         // 位移距离（像素）
                Duration = 1,           // 位移动画持续时间（秒）
                Delay = 0,              // 位移动画延迟时间（秒）
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),  // 位移动画曲线
                CurveName = "",         // 曲线库中对应的曲线名称
                Ease = EaseMode.InOutCubic  // 缓动模式
            },
            Rotation = new MotionNode_Rotation
            {
                Rotation = HudMotion_Rotation.A_无旋转,  // 旋转方向类型
                Degree = 0,             // 旋转角度（度）
                Duration = 1,           // 旋转动画持续时间（秒）
                Delay = 0,              // 旋转动画延迟时间（秒）
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),  // 旋转动画曲线
                CurveName = "",         // 曲线库中对应的曲线名称
                Ease = EaseMode.InOutCubic  // 缓动模式
            }
        };

        /// <summary>
        /// 回收动效参数名称
        /// 用于从动效库中加载预设的回收动效配置
        /// </summary>
        public string RecycleParamName;

        #endregion

        #region 位置与变换参数
        /// <summary>
        /// 元素尺寸
        /// 设置生成元素的宽高尺寸（像素）
        /// </summary>
        public Vector2 ElementSize;
        /// <summary>
        /// 元素偏移量
        /// 相对于生成位置的偏移量（本地坐标偏移）
        /// </summary>
        public Vector3 ElementOffset;
        /// <summary>
        /// 元素缩放
        /// 设置生成元素的本地缩放比例
        /// </summary>
        public Vector3 ElementScale = new Vector3(1, 1, 1);
        /// <summary>
        /// 世界空间位置
        /// 当 Opt_WorldCreate 为 true 且无参考对象时，使用此世界坐标位置生成元素
        /// </summary>
        public Vector3 WorldPosition;
        /// <summary>
        /// 世界空间旋转
        /// 当 Opt_WorldCreate 为 true 且无参考对象时，使用此世界旋转角度生成元素
        /// </summary>
        public Vector3 WorldRotation;
        /// <summary>
        /// 世界空间缩放
        /// 当 Opt_WorldCreate 为 true 且无参考对象时，使用此世界缩放比例生成元素
        /// </summary>
        public Vector3 WorldScale = new Vector3(1, 1, 1);
        /// <summary>
        /// 参考对象
        /// 当 Opt_WorldCreate 为 true 时，元素的位置、旋转、缩放将参考此 Transform
        /// </summary>
        public Transform ReferObject;
        #endregion

        #region 操作配置
        /// <summary>
        /// 主功能键
        /// 生成/回收操作的主修饰键，需要同时按下此键和触发键才可执行操作
        /// </summary>
        public SpawnFunctionKey SpawnFunctionKey_Primary = SpawnFunctionKey.Shift;
        /// <summary>
        /// 辅助功能键
        /// 生成/回收操作的辅助修饰键，需要同时按下主键、辅助键和触发键才可执行操作
        /// </summary>
        public SpawnFunctionKey SpawnFunctionKey_Secondary = SpawnFunctionKey.None;
        /// <summary>
        /// 生成触发键
        /// 按下此键（配合功能键）触发元素生成操作
        /// </summary>
        public KeyCode Key_Create = KeyCode.G;
        /// <summary>
        /// 回收触发键
        /// 按下此键（配合功能键）触发元素回收操作
        /// </summary>
        public KeyCode Key_Recycle = KeyCode.R;
        /// <summary>
        /// 手动生成开关
        /// 是否允许通过快捷键手动生成和回收元素
        /// </summary>
        public bool ManullyCreate = true;
        /// <summary>
        /// 可视化生成开关
        /// 是否在编辑器模式下自动生成元素进行可视化预览
        /// </summary>
        public bool VisuallerCreate;
        /// <summary>
        /// 世界空间生成模式
        /// true：在世界空间生成 UI 元素（3D UI）；false：在屏幕空间生成 UI 元素
        /// </summary>
        public bool WorldCreate;
        /// <summary>
        /// RMS 布局适配开关
        /// 是否启用响应式布局系统（Responsive Layout System）
        /// </summary>
        public bool RMSEnabled;
        /// <summary>
        /// 自动播放进入动画
        /// 元素生成后是否自动播放进入动画
        /// </summary>
        public bool AutoIn = true;
        [SerializeField]
        /// <summary>
        /// 使用元素自身动效
        /// </summary>
        public bool UseElementSelfMotion = true;
        #endregion

        #region 运行时状态

        [SerializeField]
        private bool m_SpawnerRunning;
        [SerializeField]
        /// <summary>
        /// 操作运行状态
        /// 标识当前生成器是否正在执行生成或回收操作（播放动画中）
        /// 当值为 true 时，新的操作将被阻止，防止动画冲突
        /// </summary>
        public bool IsSpawing
        {
            get
            {
                return m_SpawnerRunning;
            }
            set
            {
                if (value != m_SpawnerRunning)
                {
                    m_SpawnerRunning = value;

                    if (act_on_element_state_changed != null)
                        act_on_element_state_changed(value);

                    if (eve_on_element_state_changed != null)
                    {
                        eve_on_element_state_changed.Invoke(value);
                    }
                }
            }
        }
        #endregion

        #region 库名称配置
        [SerializeField]
        /// <summary>
        /// 生成动效库名称
        /// 用于从动效库中加载预设的生成动效模板
        /// </summary>
        public string Crc_Lib_Name;
        [SerializeField]
        /// <summary>
        /// 回收动效库名称
        /// 用于从动效库中加载预设的回收动效模板
        /// </summary>
        public string Rec_Lib_Name;
        [SerializeField]
        /// <summary>
        /// RMS 布局方案名称
        /// 指定使用的响应式布局方案名称
        /// </summary>
        public string RMS_SelctedName;
        #endregion

        #region UI 折叠状态
        [SerializeField]
        /// <summary>
        /// 回收动效 - 位移参数折叠状态（编辑器使用）
        /// 控制 Inspector 面板中回收动效位移参数区域的展开/折叠状态
        /// </summary>
        public bool recycle_fold_move;
        [SerializeField]
        /// <summary>
        /// 回收动效 - 旋转参数折叠状态（编辑器使用）
        /// 控制 Inspector 面板中回收动效旋转参数区域的展开/折叠状态
        /// </summary>
        public bool recycle_fold_rotate;
        [SerializeField]
        /// <summary>
        /// 回收动效 - 透明度参数折叠状态（编辑器使用）
        /// 控制 Inspector 面板中回收动效透明度参数区域的展开/折叠状态
        /// </summary>
        public bool recycle_fold_alpha;
        [SerializeField]
        /// <summary>
        /// 生成动效 - 位移参数折叠状态（编辑器使用）
        /// 控制 Inspector 面板中生成动效位移参数区域的展开/折叠状态
        /// </summary>
        public bool create_fold_move;
        [SerializeField]
        /// <summary>
        /// 生成动效 - 旋转参数折叠状态（编辑器使用）
        /// 控制 Inspector 面板中生成动效旋转参数区域的展开/折叠状态
        /// </summary>
        public bool create_fold_rotate;
        [SerializeField]
        /// <summary>
        /// 生成动效 - 透明度参数折叠状态（编辑器使用）
        /// 控制 Inspector 面板中生成动效透明度参数区域的展开/折叠状态
        /// </summary>
        public bool create_fold_alpha;
        #endregion

        #region 操作保护
        [SerializeField]
        /// <summary>
        /// 操作保护开关
        /// 防止元素动效未结束时再次触发生成或回收操作，避免动画冲突
        /// </summary>
        public bool ProtectedAction = true;
        #endregion

        #region Unity 事件回调
        [SerializeField]
        /// <summary>
        /// 动作 - 元素生成完成回调
        /// 当元素成功生成并完成进入动画后触发
        /// </summary>
        public UnityAction<XHud_Module_Element> act_on_element_spawn;
        [SerializeField]
        /// <summary>
        /// 动作 - 元素回收完成回调
        /// 当元素成功回收并完成退出动画后触发
        /// </summary>
        public UnityAction<XHud_Module_Element> act_on_element_despawn;
        [SerializeField]
        /// <summary>
        /// 动作 - 元素进入动画开始回调
        /// 元素进入动画开始时触发
        /// </summary>
        public UnityAction act_on_element_in_start;
        [SerializeField]
        /// <summary>
        /// 动作 - 元素进入动画结束回调
        /// 元素进入动画结束时触发
        /// </summary>
        public UnityAction act_on_element_in_end;
        [SerializeField]
        /// <summary>
        /// 动作 - 元素退出动画开始回调
        /// 元素退出动画开始时触发
        /// </summary>
        public UnityAction act_on_element_out_start;
        [SerializeField]
        /// <summary>
        /// 动作 - 元素退出动画结束回调
        /// 元素退出动画结束时触发
        /// </summary>
        public UnityAction act_on_element_out_end;
        [SerializeField]
        /// <summary>
        /// 动作 - 元素操作状态改变回调
        /// 当生成器的运行状态（Opt_IsRunning）发生变化时触发
        /// </summary>
        public UnityAction<bool> act_on_element_state_changed;
        [SerializeField]
        /// <summary>
        /// UnityEvent - 生成元素开始事件
        /// 可通过 Inspector 面板绑定的元素生成开始事件
        /// </summary>
        public UnityEvent eve_on_element_spawn_start;
        [SerializeField]
        /// <summary>
        /// UnityEvent - 生成元素结束事件
        /// 可通过 Inspector 面板绑定的元素生成结束事件
        /// </summary>
        public UnityEvent eve_on_element_spawn_end;
        [SerializeField]
        /// <summary>
        /// UnityEvent - 回收元素开始事件
        /// 可通过 Inspector 面板绑定的元素回收开始事件
        /// </summary>
        public UnityEvent eve_on_element_despawn_start;
        [SerializeField]
        /// <summary>
        /// UnityEvent - 回收元素结束事件
        /// 可通过 Inspector 面板绑定的元素回收结束事件
        /// </summary>
        public UnityEvent eve_on_element_despawn_end;
        [SerializeField]
        /// <summary>
        /// UnityEvent - 元素操作状态改变事件
        /// 可通过 Inspector 面板绑定的状态改变事件，参数为当前运行状态
        /// </summary>
        public UnityEvent<bool> eve_on_element_state_changed;
        #endregion

        #region Unity 生命周期
        private void Start()
        {
            // 预留初始化逻辑
        }

        /// <summary>
        /// 组件启用时的处理
        /// 如果启用了可视化生成模式，则在编辑器模式下自动生成元素用于预览
        /// </summary>
        private void OnEnable()
        {
            if (VisuallerCreate)
            {
                if (XHud_Manager.Instance == null)
                    return;
                if (CheckElementLibraryIsEmpty())
                    return;
                if (SpawnElement == null)
                    SpawnElement = hsp_Spawn();
            }
        }

        /// <summary>
        /// 组件禁用时的处理
        /// 如果启用了可视化生成模式，则回收已生成的预览元素
        /// </summary>
        private void OnDisable()
        {
            if (VisuallerCreate)
            {
                if (CheckElementLibraryIsEmpty())
                    return;
                if (SpawnElement != null)
                    hsp_Despawn(RecycleArgs);
            }
        }

        /// <summary>
        /// 每帧更新，处理快捷键输入
        /// 检测功能键组合和触发键，执行生成或回收操作
        /// </summary>
        private void Update()
        {
            if (FunctionKey_Primary_Detect() && FunctionKey_Secondary_Detect())
            {
                if (Input.GetKeyDown(Key_Create))
                {
                    if (CheckElementLibraryIsEmpty())
                        return;
                    if (!ManullyCreate)
                        return;
                    if (IsSpawing)
                        return;
                    if (SpawnElement != null)
                        return;
                    SpawnElement = hsp_Spawn(SpawnIndicator, CreateArgs);
                }

                if (Input.GetKeyDown(Key_Recycle))
                {
                    if (CheckElementLibraryIsEmpty())
                        return;
                    if (!ManullyCreate)
                        return;
                    if (IsSpawing)
                        return;
                    if (SpawnElement == null)
                        return;
                    hsp_Despawn(RecycleArgs);
                }
            }
        }

        #endregion

        #region  生成 & 回收
        /// <summary>
        /// 从池中取出一个 XHud 元素并生成到场景中
        /// </summary>
        /// <param name="IndicatorName">元素标识名称，用于区分同一预制体的不同实例，为空时使用脚本自带的 SpawnIndicator</param>
        /// <param name="CreateParam">生成动效参数，为空时使用脚本自带的 CreateArgs</param>
        /// <returns>生成成功的 XHud 元素实例，失败时返回 null</returns>
        public XHud_Module_Element hsp_Spawn(string IndicatorName = "", Motion_Creator CreateParam = null)
        {
            XHud_Module_Element element = null;

            Motion_Creator arg = CreateParam == null ? this.CreateArgs : CreateParam;

            string indicator = string.IsNullOrEmpty(IndicatorName) ? SpawnIndicator : IndicatorName;
            // 从元素库中获取目标元素本体
            XHud_Module_Element target_ele = XHud_Manager.Instance.hm_ElementLibrary_GetTargetLibrary(LibName).ElementsLibrary_GetTargetElement(SpawnName);

            ///---如果UI渲染模式为世界空间则使用世界空间专用的方法
            if (WorldCreate)
            {
                ///---如果参考物体存在则生成的UI的坐标信息则参考这个物体的坐标信息
                if (ReferObject != null)
                {
                    element = XHud_Manager.Instance.hm_WorldElement_Create(LibName, SpawnName)
                      .SetAlpha(0)
                      .SetAnchored_World(indicator)
                      .SetPosition_World(ReferObject.position)
                      .SetRotation_World(Quaternion.Euler(ReferObject.eulerAngles))
                      .SetOffset(ElementOffset)
                      .SetScale(ReferObject.localScale)
                      .SetSize(ElementSize)
                      .On_In_Start((e) =>
                      {
                          ///--------当元素 - 进入 - 开始时
                          if (act_on_element_in_start != null)
                              act_on_element_in_start();
                          eve_on_element_spawn_start.Invoke();
                          if (ProtectedAction)
                              IsSpawing = true;
                      }).On_In_End((e) =>
                      {
                          ///--------当元素 - 进入 - 结束时
                          if (act_on_element_in_end != null)
                              act_on_element_in_end();
                          eve_on_element_spawn_end.Invoke();
                          if (ProtectedAction)
                              IsSpawing = false;
                      }).Element;
                }
                else
                {
                    element = XHud_Manager.Instance.hm_WorldElement_Create(LibName, SpawnName)
                        .SetAlpha(0)
                        .SetAnchored_World(indicator)
                        .SetPosition_World(WorldPosition)
                        .SetRotation_World(Quaternion.Euler(WorldRotation))
                        .SetOffset(ElementOffset)
                        .SetScale(WorldScale)
                        .SetSize(ElementSize)
                        .On_In_Start((e) =>
                        {
                            ///--------当元素 - 进入 - 开始时
                            if (act_on_element_in_start != null)
                                act_on_element_in_start();
                            eve_on_element_spawn_start.Invoke();
                            if (ProtectedAction)
                                IsSpawing = true;
                        }).On_In_End((e) =>
                        {
                            ///--------当元素 - 进入 - 结束时
                            if (act_on_element_in_end != null)
                                act_on_element_in_end();
                            eve_on_element_spawn_end.Invoke();

                            if (ProtectedAction)
                                IsSpawing = false;
                        }).Element;
                }
            }
            else
            {
                element = XHud_Manager.Instance.hm_ScreenElement_Create(LibName, SpawnName)
                    .SetAlpha(0)
                    .SetAnchored_Screen(UseElementSelfMotion ? target_ele.CreateArgs.anchor : arg.anchor, indicator)
                    .SetOffset(ElementOffset)
                    .SetScale(ElementScale)
                    .SetSize(ElementSize)
                    .SetRMS(RMSEnabled, RMS_SelctedName)
                    .On_In_Start((e) =>
                    {
                        ///--------当元素 - 进入 - 开始时
                        if (act_on_element_in_start != null)
                            act_on_element_in_start();
                        eve_on_element_spawn_start.Invoke();
                        if (ProtectedAction)
                            IsSpawing = true;
                    }).On_In_End((e) =>
                    {
                        ///--------当元素 - 进入 - 结束时
                        if (act_on_element_in_end != null)
                            act_on_element_in_end();
                        eve_on_element_spawn_end.Invoke();
                        if (ProtectedAction)
                            IsSpawing = false;
                    }).Element;
            }

            if (AutoIn)
                element.Element_In(UseElementSelfMotion ? element.CreateArgs : arg);

            if (act_on_element_spawn != null)
                act_on_element_spawn(element);
            SpawnElement = element;
            return element;
        }
        /// <summary>
        /// 回收一个 XHud 元素到对象池中
        /// </summary>
        /// <param name="RecycleParam">回收动效参数，为空时使用脚本自带的 RecycleArgs</param>
        /// <param name="actionstart">回收开始时的回调事件，用于覆盖默认的动画开始处理</param>
        /// <param name="actionend">回收结束时的回调事件，用于覆盖默认的动画结束处理</param>
        public void hsp_Despawn(Motion_Recycler RecycleParam = null, UnityAction<XHud_Module_Element> actionstart = null, UnityAction<XHud_Module_Element> actionend = null)
        {
            XHud_Manager.Instance.hm_HudElement_RecycleAt(
                SpawnElement, (UseElementSelfMotion ? SpawnElement.RecycleArgs : (RecycleParam == null ? this.RecycleArgs : RecycleParam)),
                actionstart == null ? (ele) =>
                {
                    if (act_on_element_out_start != null)
                        act_on_element_out_start();
                    eve_on_element_despawn_start.Invoke();
                    if (ProtectedAction)
                        IsSpawing = true;
                }
            : actionstart,
                null, actionend == null ? (ele) =>
                {
                    if (act_on_element_out_end != null)
                        act_on_element_out_end();
                    if (act_on_element_despawn != null)
                        act_on_element_despawn(SpawnElement);
                    eve_on_element_despawn_end.Invoke();
                    if (ProtectedAction)
                    {
                        SpawnElement = null;
                        IsSpawing = false;
                    }
                }
            : actionend);

            if (!ProtectedAction)
            {
                SpawnElement = null;
            }
        }
        /// <summary>
        /// 检查元素库是否为空
        /// </summary>
        /// <returns>true 表示元素库为空或未初始化，false 表示元素库可用</returns>
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
        #endregion

        #region 私有方法
        /// <summary>
        /// 检测主功能键是否按下
        /// </summary>
        /// <returns>true 表示主功能键条件满足，false 表示不满足</returns>
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
        /// <returns>true 表示辅助功能键条件满足，false 表示不满足</returns>
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
        #endregion
    }
}