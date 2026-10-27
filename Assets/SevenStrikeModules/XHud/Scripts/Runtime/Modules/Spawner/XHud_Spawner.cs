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
        public string target_library_name;
        [SerializeField]
        /// <summary>
        /// 生成元素名称标识
        /// 用于在元素库中查找对应的预制体名称
        /// </summary>
        public string target_spawn_name;
        [SerializeField]
        /// <summary>
        /// 生成元素标识符
        /// 用于区分同一预制体的不同实例，可自定义标识名称
        /// </summary>
        public string target_spawn_indicator;
        [SerializeField]
        /// <summary>
        /// 生成器自身标识符
        /// 用于标识当前生成器实例，便于管理和调试
        /// </summary>
        public string spawner_indicator;
        [SerializeField]
        /// <summary>
        /// 当前生成的元素实例
        /// 保存通过此生成器创建的元素对象引用
        /// </summary>
        public XHud_Module_Element spawned_element;
        #endregion

        #region 折叠
        public bool
            fold_param = true,
            fold_trans = true,
            fold_trans_world = true,
            fold_trans_world_refer = true,
            fold_motion = true,
            fold_state = true,
            fold_option = true,
            fold_target = true,
            fold_based = true,
            fold_rms = true;

        public void GroupFold(bool state)
        {
            fold_param = state;
            fold_trans = state;
            fold_trans_world = state;
            fold_trans_world_refer = state;
            fold_motion = state;
            fold_state = state;
            fold_option = state;
            fold_target = state;
            fold_based = state;
            fold_rms = state;
        }

        public void GroupFold(bool param = true, bool trans = true, bool trans_world = true, bool trans_world_refer = true, bool motion = true, bool state = true, bool option = true, bool selector = true, bool based = true, bool rms = true)
        {
            fold_param = param;
            fold_trans = trans;
            fold_trans_world = trans_world;
            fold_trans_world_refer = trans_world_refer;
            fold_motion = motion;
            fold_state = state;
            fold_option = option;
            fold_target = selector;
            fold_based = based;
            fold_rms = rms;
        }
        #endregion

        #region 动效参数
        [SerializeField]
        /// <summary>
        /// 元素生成动效参数
        /// 定义元素进入/显示时的动画效果，包括位移、旋转、透明度等
        /// 支持缓动曲线、持续时间、延迟等参数配置
        /// </summary>
        public Motion_Creator motion_create_args = new Motion_Creator
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
        /// 元素回收动效参数
        /// 定义元素退出/隐藏时的动画效果，包括位移、旋转、透明度等
        /// 支持缓动曲线、持续时间、延迟等参数配置
        /// </summary>
        public Motion_Recycler motion_recycle_args = new Motion_Recycler
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
        #endregion

        #region 位置与变换参数
        /// <summary>
        /// 元素尺寸
        /// 设置生成元素的宽高尺寸（像素）
        /// </summary>
        public Vector2 preset_element_size;
        /// <summary>
        /// 元素偏移量
        /// 相对于生成位置的偏移量（本地坐标偏移）
        /// </summary>
        public Vector3 preset_element_offset;
        /// <summary>
        /// 元素缩放
        /// 设置生成元素的本地缩放比例
        /// </summary>
        public Vector3 preset_element_scale = new Vector3(1, 1, 1);
        /// <summary>
        /// 世界空间位置
        /// 当 Opt_WorldCreate 为 true 且无参考对象时，使用此世界坐标位置生成元素
        /// </summary>
        public Vector3 preset_element_world_position;
        /// <summary>
        /// 世界空间旋转
        /// 当 Opt_WorldCreate 为 true 且无参考对象时，使用此世界旋转角度生成元素
        /// </summary>
        public Vector3 preset_element_world_rotation;
        /// <summary>
        /// 世界空间缩放
        /// 当 Opt_WorldCreate 为 true 且无参考对象时，使用此世界缩放比例生成元素
        /// </summary>
        public Vector3 preset_element_world_scale = new Vector3(1, 1, 1);
        /// <summary>
        /// 参考对象
        /// 当 Opt_WorldCreate 为 true 时，元素的位置、旋转、缩放将参考此 Transform
        /// </summary>
        public Transform preset_element_referobject;
        #endregion

        #region 操作配置
        [SerializeField]
        /// <summary>
        /// 主功能键
        /// 生成/回收操作的主修饰键，需要同时按下此键和触发键才可执行操作
        /// </summary>
        public SpawnFunctionKey spawn_key_primary = SpawnFunctionKey.shift;
        [SerializeField]
        /// <summary>
        /// 辅助功能键
        /// 生成/回收操作的辅助修饰键，需要同时按下主键、辅助键和触发键才可执行操作
        /// </summary>
        public SpawnFunctionKey spawn_key_secondary = SpawnFunctionKey.none;
        [SerializeField]
        /// <summary>
        /// 生成触发键
        /// 按下此键（配合功能键）触发元素生成操作
        /// </summary>
        public KeyCode spawn_key_create = KeyCode.G;
        [SerializeField]
        /// <summary>
        /// 回收触发键
        /// 按下此键（配合功能键）触发元素回收操作
        /// </summary>
        public KeyCode spawn_key_recycle = KeyCode.R;
        [SerializeField]
        /// <summary>
        /// 手动生成开关
        /// 是否允许通过快捷键手动生成和回收元素
        /// </summary>
        public bool opt_create_manully = true;
        [SerializeField]
        /// <summary>
        /// 可视化生成开关
        /// 是否在编辑器模式下自动生成元素进行可视化预览
        /// </summary>
        public bool opt_create_visual;
        [SerializeField]
        /// <summary>
        /// 世界空间生成模式
        /// true：在世界空间生成 UI 元素（3D UI）；false：在屏幕空间生成 UI 元素
        /// </summary>
        public bool opt_create_world;
        [SerializeField]
        /// <summary>
        /// RMS 布局适配开关
        /// 是否启用响应式布局系统（Responsive Layout System）
        /// </summary>
        public bool opt_rms_mode;
        [SerializeField]
        /// <summary>
        /// 自动播放进入动画
        /// 元素生成后是否自动播放进入动画
        /// </summary>
        public bool opt_auto_in = true;
        [SerializeField]
        /// <summary>
        /// 使用元素自身动效
        /// </summary>
        public bool opt_use_element_motion = true;
        #endregion

        #region 运行时状态
        [SerializeField]
        private bool m_opt_is_spawing;
        public const string editor_serializefield_m_opt_is_spawing = nameof(m_opt_is_spawing);
        [SerializeField]
        /// <summary>
        /// 操作运行状态
        /// 标识当前生成器是否正在执行生成或回收操作（播放动画中）
        /// 当值为 true 时，新的操作将被阻止，防止动画冲突
        /// </summary>
        public bool opt_is_spawing
        {
            get
            {
                return m_opt_is_spawing;
            }
            set
            {
                if (value != m_opt_is_spawing)
                {
                    m_opt_is_spawing = value;

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
        public string motion_create_library_index_name;
        [SerializeField]
        /// <summary>
        /// 回收动效库名称
        /// 用于从动效库中加载预设的回收动效模板
        /// </summary>
        public string motion_recycle_library_index_name;
        [SerializeField]
        /// <summary>
        /// RMS 布局方案名称
        /// 指定使用的响应式布局方案名称
        /// </summary>
        public string rms_index_name;
        #endregion

        #region 操作保护
        [SerializeField]
        /// <summary>
        /// 操作保护开关
        /// 防止元素动效未结束时再次触发生成或回收操作，避免动画冲突
        /// </summary>
        public bool opt_protected_spawn_action = true;
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
            if (opt_create_visual)
            {
                if (XHud_Manager.instance == null)
                    return;
                if (hsp_check_element_library_is_empty())
                    return;
                if (spawned_element == null)
                    spawned_element = hsp_spawn();
            }
        }

        /// <summary>
        /// 组件禁用时的处理
        /// 如果启用了可视化生成模式，则回收已生成的预览元素
        /// </summary>
        private void OnDisable()
        {
            if (opt_create_visual)
            {
                if (hsp_check_element_library_is_empty())
                    return;
                if (spawned_element != null)
                    hsp_despawn(motion_recycle_args);
            }
        }

        /// <summary>
        /// 每帧更新，处理快捷键输入
        /// 检测功能键组合和触发键，执行生成或回收操作
        /// </summary>
        private void Update()
        {
            if (functionkey_primary_detect() && functionkey_secondary_detect())
            {
                if (Input.GetKeyDown(spawn_key_create))
                {
                    if (hsp_check_element_library_is_empty())
                        return;
                    if (!opt_create_manully)
                        return;
                    if (opt_is_spawing)
                        return;
                    if (spawned_element != null)
                        return;
                    spawned_element = hsp_spawn(target_spawn_indicator, motion_create_args);
                }

                if (Input.GetKeyDown(spawn_key_recycle))
                {
                    if (hsp_check_element_library_is_empty())
                        return;
                    if (!opt_create_manully)
                        return;
                    if (opt_is_spawing)
                        return;
                    if (spawned_element == null)
                        return;
                    hsp_despawn(motion_recycle_args);
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
        public XHud_Module_Element hsp_spawn(string IndicatorName = "", Motion_Creator CreateParam = null)
        {
            XHud_Module_Element element = null;

            Motion_Creator arg = CreateParam == null ? this.motion_create_args : CreateParam;

            string indicator = string.IsNullOrEmpty(IndicatorName) ? target_spawn_indicator : IndicatorName;
            // 从元素库中获取目标元素本体
            XHud_Module_Element target_ele = XHud_Manager.instance.hm_ElementLibrary_GetTargetLibrary(target_library_name).ElementsLibrary_GetTargetElement(target_spawn_name);

            ///---如果UI渲染模式为世界空间则使用世界空间专用的方法
            if (opt_create_world)
            {
                ///---如果参考物体存在则生成的UI的坐标信息则参考这个物体的坐标信息
                if (preset_element_referobject != null)
                {
                    element = XHud_Manager.instance.hm_WorldElement_Create(target_library_name, target_spawn_name)
                      .SetAlpha(0)
                      .SetAnchored_World(indicator)
                      .SetPosition_World(preset_element_referobject.position)
                      .SetRotation_World(Quaternion.Euler(preset_element_referobject.eulerAngles))
                      .SetOffset(preset_element_offset)
                      .SetScale(preset_element_referobject.localScale)
                      .SetSize(preset_element_size)
                      .On_In_Start((e) =>
                      {
                          ///--------当元素 - 进入 - 开始时
                          if (act_on_element_in_start != null)
                              act_on_element_in_start();
                          eve_on_element_spawn_start.Invoke();
                          if (opt_protected_spawn_action)
                              opt_is_spawing = true;
                      }).On_In_End((e) =>
                      {
                          ///--------当元素 - 进入 - 结束时
                          if (act_on_element_in_end != null)
                              act_on_element_in_end();
                          eve_on_element_spawn_end.Invoke();
                          if (opt_protected_spawn_action)
                              opt_is_spawing = false;
                      }).Element;
                }
                else
                {
                    element = XHud_Manager.instance.hm_WorldElement_Create(target_library_name, target_spawn_name)
                        .SetAlpha(0)
                        .SetAnchored_World(indicator)
                        .SetPosition_World(preset_element_world_position)
                        .SetRotation_World(Quaternion.Euler(preset_element_world_rotation))
                        .SetOffset(preset_element_offset)
                        .SetScale(preset_element_world_scale)
                        .SetSize(preset_element_size)
                        .On_In_Start((e) =>
                        {
                            ///--------当元素 - 进入 - 开始时
                            if (act_on_element_in_start != null)
                                act_on_element_in_start();
                            eve_on_element_spawn_start.Invoke();
                            if (opt_protected_spawn_action)
                                opt_is_spawing = true;
                        }).On_In_End((e) =>
                        {
                            ///--------当元素 - 进入 - 结束时
                            if (act_on_element_in_end != null)
                                act_on_element_in_end();
                            eve_on_element_spawn_end.Invoke();

                            if (opt_protected_spawn_action)
                                opt_is_spawing = false;
                        }).Element;
                }
            }
            else
            {
                element = XHud_Manager.instance.hm_ScreenElement_Create(target_library_name, target_spawn_name)
                    .SetAlpha(0)
                    .SetAnchored_Screen(opt_use_element_motion ? target_ele.motion_create_args.anchor : arg.anchor, indicator)
                    .SetOffset(preset_element_offset)
                    .SetScale(preset_element_scale)
                    .SetSize(preset_element_size)
                    .SetRMS(opt_rms_mode, rms_index_name)
                    .On_In_Start((e) =>
                    {
                        ///--------当元素 - 进入 - 开始时
                        if (act_on_element_in_start != null)
                            act_on_element_in_start();
                        eve_on_element_spawn_start.Invoke();
                        if (opt_protected_spawn_action)
                            opt_is_spawing = true;
                    }).On_In_End((e) =>
                    {
                        ///--------当元素 - 进入 - 结束时
                        if (act_on_element_in_end != null)
                            act_on_element_in_end();
                        eve_on_element_spawn_end.Invoke();
                        if (opt_protected_spawn_action)
                            opt_is_spawing = false;
                    }).Element;
            }

            if (opt_auto_in)
                element.Element_In(opt_use_element_motion ? element.motion_create_args : arg);

            if (act_on_element_spawn != null)
                act_on_element_spawn(element);
            spawned_element = element;
            return element;
        }
        /// <summary>
        /// 回收一个 XHud 元素到对象池中
        /// </summary>
        /// <param name="RecycleParam">回收动效参数，为空时使用脚本自带的 RecycleArgs</param>
        /// <param name="actionstart">回收开始时的回调事件，用于覆盖默认的动画开始处理</param>
        /// <param name="actionend">回收结束时的回调事件，用于覆盖默认的动画结束处理</param>
        public void hsp_despawn(Motion_Recycler RecycleParam = null, UnityAction<XHud_Module_Element> actionstart = null, UnityAction<XHud_Module_Element> actionend = null)
        {
            XHud_Manager.instance.hm_HudElement_RecycleAt(
                spawned_element, (opt_use_element_motion ? spawned_element.motion_recycle_args : (RecycleParam == null ? this.motion_recycle_args : RecycleParam)),
                actionstart == null ? (ele) =>
                {
                    if (act_on_element_out_start != null)
                        act_on_element_out_start();
                    eve_on_element_despawn_start.Invoke();
                    if (opt_protected_spawn_action)
                        opt_is_spawing = true;
                }
            : actionstart,
                null, actionend == null ? (ele) =>
                {
                    if (act_on_element_out_end != null)
                        act_on_element_out_end();
                    if (act_on_element_despawn != null)
                        act_on_element_despawn(spawned_element);
                    eve_on_element_despawn_end.Invoke();
                    if (opt_protected_spawn_action)
                    {
                        spawned_element = null;
                        opt_is_spawing = false;
                    }
                }
            : actionend);

            if (!opt_protected_spawn_action)
            {
                spawned_element = null;
            }
        }
        /// <summary>
        /// 检查元素库是否为空
        /// </summary>
        /// <returns>true 表示元素库为空或未初始化，false 表示元素库可用</returns>
        public bool hsp_check_element_library_is_empty()
        {
            bool sw = false;
            if (!Application.isPlaying)
            {
                XHud_Manager mgr = FindFirstObjectByType<XHud_Manager>();

                if (mgr.ele_library_elements == null)
                    sw = true;
                if (mgr.ele_library_elements.Count <= 0)
                    sw = true;
            }
            else
            {
                if (XHud_Manager.instance.ele_library_elements == null)
                    sw = true;
                if (XHud_Manager.instance.ele_library_elements.Count <= 0)
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
        private bool functionkey_primary_detect()
        {
            bool ispress = false;
            switch (spawn_key_primary)
            {
                case SpawnFunctionKey.none:
                    ispress = true;
                    break;
                case SpawnFunctionKey.ctrl:
                    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                        ispress = true;
                    break;
                case SpawnFunctionKey.alt:
                    if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                        ispress = true;
                    break;
                case SpawnFunctionKey.shift:
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
        private bool functionkey_secondary_detect()
        {
            bool ispress = false;
            switch (spawn_key_secondary)
            {
                case SpawnFunctionKey.none:
                    ispress = true;
                    break;
                case SpawnFunctionKey.ctrl:
                    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                        ispress = true;
                    break;
                case SpawnFunctionKey.alt:
                    if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                        ispress = true;
                    break;
                case SpawnFunctionKey.shift:
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                        ispress = true;
                    break;
            }
            return ispress;
        }
        #endregion
    }
}