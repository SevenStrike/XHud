/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XTween - Unity 高性能动画架构插件
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
namespace SevenStrikeModules.XTween
{
    using System;
    using UnityEngine;

    [Serializable]
    /// <summary>
    /// XTween配置数据类
    /// </summary>
    public class ConfigDatas
    {
        /// <summary>
        /// 主题色
        /// </summary>
        public Color Theme_Primary;
        /// <summary>
        /// 群组色
        /// </summary>
        public Color Theme_Group;
        /// <summary>
        /// 分割线颜色
        /// </summary>
        public Color Theme_SeperateLine;
        /// <summary>
        /// 液晶显示扫描线风格
        /// </summary>
        public bool LiquidScanStyle;
        /// <summary>
        /// 液晶显示肮脏效果
        /// </summary>
        public bool LiquidDirty;
        /// <summary>
        /// 液晶显示闪烁特效
        /// </summary>
        public int LiquidBlinker;
        /// <summary>
        /// 液晶颜色 - 播放动画
        /// </summary>
        public Color LiquidColor_Playing;
        /// <summary>
        /// 液晶颜色 - 待命中
        /// </summary>
        public Color LiquidColor_Idle;
        [SerializeField]
        /// <summary>
        /// 动画池 - Int
        /// </summary>
        public int PoolCount_Int;
        [SerializeField]
        /// <summary>
        /// 动画池 - Float
        /// </summary>
        public int PoolCount_Float;
        [SerializeField]
        /// <summary>
        /// 动画池 - String
        /// </summary>
        public int PoolCount_String;
        [SerializeField]
        /// <summary>
        /// 动画池 - Vector2
        /// </summary>
        public int PoolCount_Vector2;
        [SerializeField]
        /// <summary>
        /// 动画池 - Vector3
        /// </summary>
        public int PoolCount_Vector3;
        [SerializeField]
        /// <summary>
        /// 动画池 - Vector4
        /// </summary>
        public int PoolCount_Vector4;
        [SerializeField]
        /// <summary>
        /// 动画池 - Quaternion
        /// </summary>
        public int PoolCount_Quaternion;
        [SerializeField]
        /// <summary>
        /// 动画池 - Color
        /// </summary>
        public int PoolCount_Color;
        /// <summary>
        /// 动画池 - 场景卸载时回收所有
        /// </summary>
        public bool PoolRecyleAllOnSceneUnloaded;
        /// <summary>
        /// 动画池 - 场景加载时回收所有
        /// </summary>
        public bool PoolRecyleAllOnSceneLoaded;
        /// <summary>
        /// 极简化动画状态面板
        /// </summary>
        public bool PerformanceLiquidMode;
        /// <summary>
        /// 预览选项 - 自动杀死预览
        /// </summary>
        public bool PreviewOption_AutoKillPreviewTweens;
        /// <summary>
        /// 预览选项 - 杀死后自动倒退
        /// </summary>
        public bool PreviewOption_RewindPreviewTweensWithKill;
        /// <summary>
        /// 预览选项 - 杀死后清除预览
        /// </summary>
        public bool PreviewOption_ClearPreviewTweensWithKill;
        /// <summary>
        /// 预设模式：星标模式
        /// </summary>
        public bool PresetInFavouriteMode;
        /// <summary>
        /// 最后选择的预设类型
        /// </summary>
        public string PresetSelectionMark_LastTypeName;
        /// <summary>
        /// 预设类型光标的最后一次坐标
        /// </summary>
        public Rect PresetSelectionMark_LastRect;
        /// <summary>
        /// 预设中心窗口尺寸
        /// </summary>
        public Vector2 PresetCentralWindowSize;

        public ConfigDatas Clone()
        {
            ConfigDatas dat = new ConfigDatas();
            dat.Theme_Primary = Theme_Primary;
            dat.Theme_Group = Theme_Group;
            dat.Theme_SeperateLine = Theme_SeperateLine;
            dat.LiquidScanStyle = LiquidScanStyle;
            dat.LiquidDirty = LiquidDirty;
            dat.LiquidBlinker = LiquidBlinker;
            dat.LiquidColor_Playing = LiquidColor_Playing;
            dat.LiquidColor_Idle = LiquidColor_Idle;
            dat.PoolCount_Int = PoolCount_Int;
            dat.PoolCount_Float = PoolCount_Float;
            dat.PoolCount_String = PoolCount_String;
            dat.PoolCount_Vector2 = PoolCount_Vector2;
            dat.PoolCount_Vector3 = PoolCount_Vector3;
            dat.PoolCount_Vector4 = PoolCount_Vector4;
            dat.PoolCount_Quaternion = PoolCount_Quaternion;
            dat.PoolCount_Color = PoolCount_Color;
            dat.PoolRecyleAllOnSceneUnloaded = PoolRecyleAllOnSceneUnloaded;
            dat.PoolRecyleAllOnSceneLoaded = PoolRecyleAllOnSceneLoaded;
            dat.PerformanceLiquidMode = PerformanceLiquidMode;
            dat.PreviewOption_AutoKillPreviewTweens = PreviewOption_AutoKillPreviewTweens;
            dat.PreviewOption_RewindPreviewTweensWithKill = PreviewOption_RewindPreviewTweensWithKill;
            dat.PreviewOption_ClearPreviewTweensWithKill = PreviewOption_ClearPreviewTweensWithKill;
            dat.PresetInFavouriteMode = PresetInFavouriteMode;
            dat.PresetSelectionMark_LastTypeName = PresetSelectionMark_LastTypeName;
            dat.PresetSelectionMark_LastRect = PresetSelectionMark_LastRect;
            dat.PresetCentralWindowSize = PresetCentralWindowSize;

            return dat;
        }
    }

    public class XTween_Config : ScriptableObject
    {
        [SerializeField]
        public ConfigDatas Datas;
    }
}
