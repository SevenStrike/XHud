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
    using UnityEngine;
    using UnityEngine.Events;

    public partial class XHud_Manager : MonoBehaviour
    {
        /// <summary>
        /// 委托事件 - 元素库初始化后
        /// </summary>
        public UnityAction Act_ElementsLib_Instantiated;
        /// <summary>
        /// 委托事件 - 遮罩透明度变化时
        /// </summary>
        public UnityAction<float> Act_MaskChanged_Value;
        /// <summary>
        /// 委托事件 - 散焦遮罩透明度变化时
        /// </summary>
        public UnityAction<float> Act_BlurMaskChanged_Value;
        /// <summary>
        /// 委托事件 - 散焦特性强度最大时
        /// </summary>
        public UnityAction<float> Act_BlurMask_Intensity_IsMax;
        /// <summary>
        /// 委托事件 - 散焦特性强度最小时
        /// </summary>
        public UnityAction<float> Act_BlurMask_Intensity_IsMin;
        /// <summary>
        /// 委托事件 - 屏幕空间的内容的透明度最大时
        /// </summary>
        public UnityAction<float> Act_ContentOpacity_Screen_IsMax;
        /// <summary>
        /// 委托事件 - 屏幕空间的内容的透明度最小时
        /// </summary>
        public UnityAction<float> Act_ContentOpacity_Screen_IsMin;
        /// <summary>
        /// 委托事件 - 屏幕空间的内容的透明度变化时
        /// </summary>
        public UnityAction<float> Act_ContentOpacity_Screen_Changed;
        /// <summary>
        /// 委托事件 - 屏幕空间的内容的透明度最大时
        /// </summary>
        public UnityAction<float> Act_ContentOpacity_World_IsMax;
        /// <summary>
        /// 委托事件 - 屏幕空间的内容的透明度最小时
        /// </summary>
        public UnityAction<float> Act_ContentOpacity_World_IsMin;
        /// <summary>
        /// 委托事件 - 屏幕空间的内容的透明度变化时
        /// </summary>
        public UnityAction<float> Act_ContentOpacity_World_Changed;
        /// <summary>
        /// 委托事件 - 遮罩贴图变化时
        /// </summary>
        public UnityAction<Texture2D> Act_MaskChanged_Texture;
        /// <summary>
        /// 委托事件 - 散焦遮罩贴图变化时
        /// </summary>
        public UnityAction<Texture2D> Act_BlurMaskChanged_Texture;
        /// <summary>
        /// 委托事件 - 蓝图视觉进场
        /// </summary>
        public UnityAction Act_BluePrint_In;
        /// <summary>
        /// 委托事件 - 蓝图视觉退场
        /// </summary>
        public UnityAction Act_BluePrint_Out;
        /// <summary>
        /// 委托事件 - 蓝图视觉整体完全透明化
        /// </summary>
        public UnityAction Act_BluePrint_IsTransparency;
        /// <summary>
        /// 委托事件 - 蓝图视觉整体完全实心化
        /// </summary>
        public UnityAction Act_BluePrint_IsSolid;
        /// <summary>
        /// 委托事件 - 字体缩放变化
        /// </summary>
        public UnityAction Act_FontSize_Changed;
        /// <summary>
        /// 委托事件 - 分辨率变化
        /// </summary>
        public UnityAction<string, Vector2> Act_ScreenResolution_Changed;
        /// <summary>
        /// 委托事件 - 鼠标是否正在与UI交互
        /// </summary>
        public UnityAction<bool> Act_IsInteractionUI;
        /// <summary>
        /// 委托事件 - 全局音效音量变化时
        /// </summary>
        public UnityAction<float> Act_VolumeChanged_Value;
        /// <summary>
        /// 委托事件 - 全局速率变化时
        /// </summary>
        public UnityAction<float> Act_DurationChanged_Value;
        /// <summary>
        /// 委托事件 - 全局字体尺寸变化时
        /// </summary>
        public UnityAction<float> Act_FontSizeChanged_Value;
        /// <summary>
        /// 委托事件 - 生成元素时
        /// </summary>
        public UnityAction<XHud_Module_Element> Act_SpawnElement;
    }
}