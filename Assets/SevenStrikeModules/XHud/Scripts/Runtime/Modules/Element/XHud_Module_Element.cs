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
    using UnityEngine;

    [System.Serializable]
    [RequireComponent(typeof(CanvasGroup))]
    public partial class XHud_Module_Element : MonoBehaviour
    {
        #region 参数
        /// <summary>
        ///  ID编号
        /// </summary>
        private int id;
        [SerializeField]
        /// <summary>
        /// ID编号
        /// </summary>
        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
        [SerializeField]
        /// <summary>
        /// 元素昵称
        /// </summary>
        public string Indicator;
        [SerializeField]
        /// <summary>
        /// 元素原始名称
        /// </summary>
        public string OriginalName;
        [SerializeField]
        /// <summary>
        /// 源自元素池的名称
        /// </summary>
        public string OriginPoolName;
        [SerializeField]
        /// <summary>
        /// 调试开关
        /// </summary>
        public bool DebugState;
        [SerializeField]
        /// <summary>
        /// 变换组件
        /// </summary>
        public RectTransform RectTransform;
        [SerializeField]
        /// <summary>
        /// 锚点坐标
        /// </summary>
        public Vector2 CurrentPivot;
        #endregion

        #region 状态
        [SerializeField]
        /// <summary>
        /// 元素的生成状态
        /// </summary>
        public XHudElementCreateState CreateState = XHudElementCreateState.Recycled;
        [SerializeField]
        /// <summary>
        /// 元素的动画状态
        /// </summary>
        public XHudElementAnimateState AnimateState = XHudElementAnimateState.Static;
        [SerializeField]
        /// <summary>
        /// 判定元素是屏幕类型还是世界空间类型
        /// </summary>
        public XHudElementSpaceType ElementSpaceType = XHudElementSpaceType.None;
        [SerializeField]
        /// <summary>
        /// 判定元素生成源是从元素库还是自定义实例化
        /// </summary>
        public XHudElementCreatedSourceType CreatedSourceType = XHudElementCreatedSourceType.Library;
        [SerializeField]
        /// <summary>
        /// Tween动画预览状态
        /// true表示当前正在预览Tween动画
        /// false表示未在预览状态
        /// 用于编辑器模式下的动画预览控制
        /// </summary>
        public bool Tween_Preview_IsPlaying = false;
        #endregion

        private void Awake()
        {
            // 获取自身的 RectTransform
            if (RectTransform == null)
                RectTransform = GetComponent<RectTransform>();

            // 获取自身的 TriggerAction
            if (TriggerAction == null)
                TriggerAction = GetComponent<XHud_Element_TriggerAction>();
        }

        public virtual void OnEnable()
        {
            // 元素启用时立即开启 CanvasGroup 可交互
            element_SetInteractable(true);
            // 响应屏幕分辨率变化时元素的 RMS 布局方案的初始化
            elelemt_RMS_ScreenResolutionChanged(null, Vector2.zero);
        }

        public virtual void OnDisable()
        {

        }

        public virtual void Start()
        {
            // 注册屏幕分辨率变化时的动作
            XHud_Manager.Instance.Act_ScreenResolution_Changed += elelemt_RMS_ScreenResolutionChanged;
        }

        public virtual void Update()
        {
            // 同步透明度，将 Alpha 值赋值给 CanvasGroup 的 Alpha
            element_AlphaSyncUpdate();
        }
    }
}