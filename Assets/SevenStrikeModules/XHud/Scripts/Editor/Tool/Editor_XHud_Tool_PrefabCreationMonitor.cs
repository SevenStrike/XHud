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
namespace SevenStrikeModules.XHud.Editor
{
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using SevenStrikeModules.XHud.Enums;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// 预制体创建监听器
    /// 封装了通过拖拽创建预制体时的监听功能
    /// </summary>
    public static class Editor_XHud_Tool_PrefabCreationMonitor
    {
        // 存储待处理的预制体信息
        private static readonly Dictionary<string, float> pending_prefabs = new Dictionary<string, float>();
        private static bool isInitialized = false;

        /// <summary>
        /// 预制体创建事件，当预制体被创建时触发
        /// 参数：预制体路径
        /// </summary>
        public static event System.Action<string> on_prefab_created;

        /// <summary>
        /// 预制体创建完成事件，当预制体完全加载并可用时触发
        /// 参数：预制体游戏对象
        /// </summary>
        public static event System.Action<GameObject> on_prefab_ready;

        /// <summary>
        /// 初始化监听器，会在编辑器启动时自动调用
        /// </summary>
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;

            // 订阅资源创建事件
            AssetModificationProcessorHook.on_will_create_asset_handler += OnWillCreateAsset;

            // 订阅编辑器更新事件，用于处理延迟调用
            EditorApplication.update += OnEditorUpdate;

            // 订阅播放模式变化，避免播放时干扰
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            XGUI_Utilitys.Console("XHud 元素预制体监听工具消息", "元素预制体创建与覆盖监听器已就绪！", XGUIMsgState.确认);
        }

        /// <summary>
        /// 资源即将创建时的处理
        /// </summary>
        private static void OnWillCreateAsset(string assetPath)
        {
            if (!assetPath.EndsWith(".prefab")) return;

            XGUI_Utilitys.Console("XHud 元素预制体监听工具消息", $"检测到预制体创建: {assetPath}", XGUIMsgState.通知);

            // 记录待处理的预制体
            pending_prefabs[assetPath] = Time.realtimeSinceStartup;



            // 触发创建事件
            on_prefab_created?.Invoke(assetPath);
        }

        /// <summary>
        /// 编辑器更新循环，用于处理延迟逻辑
        /// </summary>
        private static void OnEditorUpdate()
        {
            if (pending_prefabs.Count == 0) return;

            // 需要等待至少一帧，确保资源完全创建
            List<string> readyPaths = new List<string>();

            foreach (var kvp in pending_prefabs)
            {
                string path = kvp.Key;
                float createTime = kvp.Value;

                // 等待至少 0.1 秒确保资源完全加载
                if (Time.realtimeSinceStartup - createTime > 0.1f)
                {
                    readyPaths.Add(path);
                }
            }

            // 处理就绪的预制体
            foreach (string path in readyPaths)
            {
                pending_prefabs.Remove(path);
                ProcessReadyPrefab(path);
            }
        }

        /// <summary>
        /// 处理已就绪的预制体
        /// </summary>
        private static void ProcessReadyPrefab(string assetPath)
        {
            try
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (prefab == null)
                {
                    XGUI_Utilitys.Console("XHud 元素预制体监听工具消息", $"无法加载预制体: {assetPath}", XGUIMsgState.错误);
                    return;
                }

                XHud_Module_Element ele = prefab.GetComponent<XHud_Module_Element>();
                if (ele != null)
                {
                    //XGUI.dialog(
                    //    type: XGUIDialogType.帮助,
                    //    windowtitle: "XHud - 元素生成器消息",
                    //    title: "推送元素入库",
                    //    msg: $"检测到您创建了元素预制体\" {ele.transform.name}{(string.IsNullOrEmpty(ele.indicator) ? "" : $" ( {ele.indicator} )")} \"，是否立即将其推送至元素库？",
                    //    ok: "推送",
                    //    cancel: "暂不",
                    //    PrimaryIndex: 0,
                    //    usemodal: true,
                    //    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    //    on_selected: (d) =>
                    //    {
                    //        if (d == "推送")
                    //        {

                    //        }
                    //    });

                    XGUI_Utilitys.Console("XHud 元素预制体监听工具消息", $"预制体已创建: {assetPath}", XGUIMsgState.确认);
                }

                on_prefab_ready?.Invoke(prefab);
            }
            catch (System.Exception e)
            {
                XGUI_Utilitys.Console("XHud 元素预制体监听工具消息", $"处理预制体时出错: {assetPath}\n{e}", XGUIMsgState.错误);
            }
        }

        /// <summary>
        /// 播放模式状态变化时的处理
        /// </summary>
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                // 退出编辑模式时，清理待处理队列
                pending_prefabs.Clear();
            }
        }

        /// <summary>
        /// 手动触发预制体处理（用于已有预制体或测试）
        /// </summary>
        /// <param name="assetPath">预制体路径</param>
        public static void ProcessPrefabManually(string assetPath)
        {
            if (!assetPath.EndsWith(".prefab"))
            {
                XGUI_Utilitys.Console("XHud 元素预制体监听工具消息", $"不是预制体文件: {assetPath}", XGUIMsgState.警告);
                return;
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab == null)
            {
                XGUI_Utilitys.Console("XHud 元素预制体监听工具消息", $"无法加载预制体: {assetPath}", XGUIMsgState.警告);
                return;
            }

            on_prefab_ready?.Invoke(prefab);
        }

        /// <summary>
        /// 清理监听器
        /// </summary>
        public static void Cleanup()
        {
            if (!isInitialized) return;

            AssetModificationProcessorHook.on_will_create_asset_handler -= OnWillCreateAsset;
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

            pending_prefabs.Clear();
            isInitialized = false;

            XGUI_Utilitys.Console("XHud 元素预制体监听工具消息", $"预制体监听器已清理！", XGUIMsgState.警告);
        }
    }

    /// <summary>
    /// 内部辅助类：用于接入 AssetModificationProcessor
    /// </summary>
    internal class AssetModificationProcessorHook : UnityEditor.AssetModificationProcessor
    {
        public static event System.Action<string> on_will_create_asset_handler;

        public static void OnWillCreateAsset(string assetName)
        {
            on_will_create_asset_handler?.Invoke(assetName);
        }
    }
}