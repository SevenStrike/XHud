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
    using UnityEditor;
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// 预制体创建监听器
    /// 封装了通过拖拽创建预制体时的监听功能
    /// </summary>
    public static class Editor_XHud_PrefabCreationMonitor
    {
        // 存储待处理的预制体信息
        private static readonly Dictionary<string, float> pendingPrefabs = new Dictionary<string, float>();
        private static bool isInitialized = false;

        /// <summary>
        /// 预制体创建事件，当预制体被创建时触发
        /// 参数：预制体路径
        /// </summary>
        public static event System.Action<string> OnPrefabCreated;

        /// <summary>
        /// 预制体创建完成事件，当预制体完全加载并可用时触发
        /// 参数：预制体游戏对象
        /// </summary>
        public static event System.Action<GameObject> OnPrefabReady;

        /// <summary>
        /// 初始化监听器，会在编辑器启动时自动调用
        /// </summary>
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;

            // 订阅资源创建事件
            AssetModificationProcessorHook.OnWillCreateAssetHandler += OnWillCreateAsset;

            // 订阅编辑器更新事件，用于处理延迟调用
            EditorApplication.update += OnEditorUpdate;

            // 订阅播放模式变化，避免播放时干扰
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            Debug.Log("[XHUD] 元素预制体创建与覆盖监听器已初始化");
        }

        /// <summary>
        /// 资源即将创建时的处理
        /// </summary>
        private static void OnWillCreateAsset(string assetPath)
        {
            if (!assetPath.EndsWith(".prefab")) return;

            Debug.Log($"[XHUD] 检测到预制体创建: {assetPath}");

            // 记录待处理的预制体
            pendingPrefabs[assetPath] = Time.realtimeSinceStartup;

            // 触发创建事件
            OnPrefabCreated?.Invoke(assetPath);
        }

        /// <summary>
        /// 编辑器更新循环，用于处理延迟逻辑
        /// </summary>
        private static void OnEditorUpdate()
        {
            if (pendingPrefabs.Count == 0) return;

            // 需要等待至少一帧，确保资源完全创建
            List<string> readyPaths = new List<string>();

            foreach (var kvp in pendingPrefabs)
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
                pendingPrefabs.Remove(path);
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
                // 加载预制体
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (prefab == null)
                {
                    Debug.LogWarning($"[XHUD] 无法加载预制体: {assetPath}");
                    return;
                }

                Debug.Log($"[XHUD] 预制体已就绪: {assetPath}");

                // 触发就绪事件
                OnPrefabReady?.Invoke(prefab);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[XHUD] 处理预制体时出错: {assetPath}\n{e}");
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
                pendingPrefabs.Clear();
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
                Debug.LogWarning($"[XHUD] 不是预制体文件: {assetPath}");
                return;
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab == null)
            {
                Debug.LogWarning($"[XHUD] 无法加载预制体: {assetPath}");
                return;
            }

            OnPrefabReady?.Invoke(prefab);
        }

        /// <summary>
        /// 清理监听器
        /// </summary>
        public static void Cleanup()
        {
            if (!isInitialized) return;

            AssetModificationProcessorHook.OnWillCreateAssetHandler -= OnWillCreateAsset;
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

            pendingPrefabs.Clear();
            isInitialized = false;

            Debug.Log("[XHUD] 预制体监听器已清理");
        }
    }

    /// <summary>
    /// 内部辅助类：用于接入 AssetModificationProcessor
    /// </summary>
    internal class AssetModificationProcessorHook : UnityEditor.AssetModificationProcessor
    {
        public static event System.Action<string> OnWillCreateAssetHandler;

        public static void OnWillCreateAsset(string assetName)
        {
            OnWillCreateAssetHandler?.Invoke(assetName);
        }
    }
}