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
    using SevenStrikeModules.XTween;
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem.UI;
    using UnityEngine.UI;

    public partial class XHud_Manager : MonoBehaviour
    {
        #region 资源库类
        [SerializeField]
        [Tooltip("曲线库")]
        /// <summary>
        /// 曲线库
        /// </summary>
        public XHud_Library_Curves Hud_Curves;
        [SerializeField]
        [Tooltip("调色板")]
        /// <summary>
        /// 调色板
        /// </summary>
        public XHud_Library_Colors Hud_Colors;
        [SerializeField]
        [Tooltip("音效库")]
        /// <summary>
        /// 音效库
        /// </summary>
        public XHud_Library_Sounds Hud_Sounds;
        [SerializeField]
        [Tooltip("字体库")]
        /// <summary>
        /// 字体库
        /// </summary>
        public XHud_Library_TextStyle Hud_TextStyleLibrary;
        [SerializeField]
        [Tooltip("转场")]
        /// <summary>
        /// 转场
        /// </summary>
        public XHud_Library_Transition Hud_TransitionLib;
        [SerializeField]
        [Tooltip("元素动效库")]
        /// <summary>
        /// 元素动效库
        /// </summary>
        public XHud_Library_Motion Hud_Motions;
        #endregion

        #region UI的全局动画速度
        [SerializeField]
        [Range(0, 1)]
        [Tooltip("影响所有UI的动画速度")]
        /// <summary>
        /// 影响所有UI的动画速度
        /// </summary>
        public float DurationMultiply = 1f;
        #endregion

        #region 必要组件
        [Tooltip("事件系统")]
        /// <summary>
        /// EventSystem组件
        /// </summary>
        public EventSystem Hud_EventSystem;
        [Tooltip("输入系统")]
        /// <summary>
        /// InputSystemUIInputModule组件
        /// </summary>
        public InputSystemUIInputModule Hud_InputSystemUIInputModule;
        [Tooltip("XTween")]
        /// <summary>
        /// XTween
        /// </summary>
        public XTween_Manager XTweenManager;
        #endregion      

        #region 状态开关
        [SerializeField]
        [Tooltip("调试模式")]
        /// <summary>
        /// 调试模式
        /// </summary>
        public bool UseDebug;
        [SerializeField]
        [Tooltip("管理器初始化状态")]
        /// <summary>
        /// 管理器初始化状态
        /// </summary>
        public bool IsInitialized;
        [SerializeField]
        /// <summary>
        /// 关闭面板后是否折叠所有选项卡
        /// </summary>
        public bool FoldAllPanelWithDisabled;
        #endregion

        public bool
            fold_options = true,
            fold_canvasset = true,
            fold_camera = true,
            fold_ratiorefer = true,
            fold_compguid = true,
            fold_reslibs = true,
            fold_elementlibs = true,
            fold_soundslib = true,
            fold_masks = true,
            fold_global = true,
            fold_tool = true,
            fold_RMS = true,
            fold_layout = true,
            fold_visualsafe = true,
            fold_blueprint = true,
            fold_motion_default = true,
            fold_themes = true,
            fold_based = true;

        [SerializeField] public float PrimtiveID_LabelLine_Height = 0.5f;
        [SerializeField] public Color PrimtiveID_LabelLine_Color = Color.white * 0.5f;
        [SerializeField] public int PrimtiveID_LabelFont_Size = 12;
        [SerializeField] public Color PrimtiveID_LabelFont_Color = Color.white;

        void Awake()
        {
            // 单例模式检查
            hm_InstanceModeCheck();
            // 启动时根据配置开关，设置屏幕画布（Canvas）的像素完美对齐模式，避免模糊
            hm_CanvasPixelPerfect_InitializeMode();

            // 在子级中获取 CanvasScaler
            if (HudCanvasScaler == null)
                HudCanvasScaler = HudCanvas_Screen.GetComponentInChildren<CanvasScaler>();

            // 在子级中获取 CustomMouseCursor
            if (Hud_MouseCursor == null)
                Hud_MouseCursor = GetComponentInChildren<XHud_CustomMouseCursor>();

            // 在子级中获取 TransitionController
            if (Hud_TransitionController == null)
                Hud_TransitionController = GetComponentInChildren<XHud_TransitionController>();

            // 根据 BluePrint_OnStartHide 配置决定蓝图视觉在游戏启动时的显示状态
            hm_BluePrint_InitializeMode();
            // 让蓝图结构永远位于底层
            hm_BluePrintRootFirstSibling();
            // 根据配置的预制体列表，预先生成指定数量的 UI 元素实例，放入对象池待用
            hm_ElementLibrary_Initialize();
            // 预创建指定数量的 AudioSource 组件，用于播放 UI 音效
            hm_LibrarySounds_Initialize(SounderPoolCount);
            // 检查 UI 交互状态的协程
            StartCoroutine(CheckUIInteraction());
        }

        private void Start()
        {

        }

        void Update()
        {
            // 获取屏幕分辨率
            hm_GetScreenResolution();
            // 更新所有已生成 Hud 元素的动画状态
            hm_Element_UpdateAnimating();
            // 负责同步屏幕遮罩（Mask）的视觉状态和射线检测属性
            hm_MaskUpdate();
            // 转场控制器更新
            hm_TransitionUpdate();
            // 同步屏幕空间 UI 内容的整体透明度，并触发相应的状态变化事件
            hm_Screen_ContentOpacity_Update();
            // 同步世界空间 UI 内容的整体透明度，并触发相应的状态变化事件
            hm_World_ContentOpacity_Update();
            // 同步音效池中每个 AudioSource 的播放状态到对应的 AudioPlayer 记录中
            hm_LibrarySounds_Update();
            // 遍历所有元素库，更新每个预生成元素的使用状态、使用计数和回收计数
            hm_ElementLibrary_UpdateStates();
            // 负责更新安全框（Safe Frame）的所有视觉元素，包括框线、分割线和中心标记
            hm_SafeFrameUpdate();
            // 负责更新所有屏幕空间锚点的位置，根据配置的边距值动态计算每个锚点的坐标
            hm_Layout_Update();
            // 同步所有相机相关配置到实际的相机组件，确保 UI 相机和场景相机的渲染设置与配置一致
            hm_CameraUpdate();
            // 负责同步蓝图视觉模式的所有视觉元素，包括整体透明度、背景、网格线、标题水印等
            hm_BluePrint_Update();
        }

        private void OnDisable()
        {
            // 清理预生成的元素队列列表
            hm_ElementLibrary_Clean();
        }

        private void OnDrawGizmos()
        {
            // 绘制屏幕构图参考线
            hm_CompGuide_Draw();
        }
    }
}