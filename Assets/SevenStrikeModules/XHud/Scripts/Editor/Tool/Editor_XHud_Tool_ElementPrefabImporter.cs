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
    /// 预制体创建监听器，封装通过拖拽创建预制体时的监听功能
    /// </summary>
    public static class Editor_XHud_Tool_ElementPrefabImporter
    {
        #region 字段 - 批处理状态
        /// <summary>
        /// 批量处理防抖计时器
        /// </summary>
        private static float batch_debounce_timer = 0;
        /// <summary>
        /// 防抖延迟时间（秒）
        /// </summary>
        private const float BATCH_DELAY = 0.24f;
        /// <summary>
        /// 待处理的预制体路径及其记录时间
        /// </summary>
        private static readonly Dictionary<string, float> pending_prefabs = new Dictionary<string, float>();
        /// <summary>
        /// 监听器是否已初始化
        /// </summary>
        private static bool isInitialized = false;
        #endregion

        #region 字段 - 事件
        /// <summary>
        /// 预制体创建事件，当预制体被创建时触发（参数为预制体路径）
        /// </summary>
        public static event System.Action<string> on_prefab_created;
        /// <summary>
        /// 预制体创建完成事件，当预制体完全加载并可用时触发（参数为预制体游戏对象）
        /// </summary>
        public static event System.Action<GameObject> on_prefab_ready;
        #endregion

        #region 初始化与清理
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
        /// 清理监听器，取消所有事件订阅并清空待处理队列
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
        #endregion

        #region 事件处理
        /// <summary>
        /// 资源即将创建时的处理：记录待处理预制体并重置防抖计时
        /// </summary>
        /// <param name="assetPath">
        /// 即将创建的资源路径
        /// </param>
        private static void OnWillCreateAsset(string assetPath)
        {
            if (!assetPath.EndsWith(".prefab")) return;

            //XGUI_Utilitys.Console("XHud 元素预制体监听工具消息", $"检测到预制体创建: {assetPath}", XGUIMsgState.通知);

            // 极速记录
            pending_prefabs[assetPath] = Time.realtimeSinceStartup;

            // 核心：每次收到新文件，就把结算时间往后推 0.3 秒
            batch_debounce_timer = Time.realtimeSinceStartup + BATCH_DELAY;

            // 触发创建事件
            on_prefab_created?.Invoke(assetPath);
        }
        /// <summary>
        /// 编辑器更新循环，在防抖计时结束后统一处理待处理的预制体
        /// </summary>
        private static void OnEditorUpdate()
        {
            if (pending_prefabs.Count == 0) return;

            // 如果还没到结算时间，说明还有文件在陆续进来，强行等！
            if (Time.realtimeSinceStartup < batch_debounce_timer)
                return;

            // 1. 提取数据并清空队列
            List<string> readyPaths = new List<string>(pending_prefabs.Keys);
            pending_prefabs.Clear();

            // 2. 将路径转换为 XHud_Module_Element 组件列表
            List<XHud_Module_Element> elements = new List<XHud_Module_Element>();
            foreach (string path in readyPaths)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    XHud_Module_Element ele = prefab.GetComponent<XHud_Module_Element>();
                    if (ele != null)
                    {
                        elements.Add(ele);
                    }
                }
            }

            if (elements.Count == 0) return;

            // 3. 弹出汇总对话框（这里做了延迟一帧处理，防止模态窗口冲突）
            int count = elements.Count;
            string msg = count == 1
                ? $"检测到您创建了元素预制体\" {elements[0].transform.name}\"，是否立即将其推送至元素库？"
                : $"检测到您刚刚批量创建了 {count} 个元素预制体，是否立即将这一批次全部推送至元素库？";

            XGUI.dialog(
                type: XGUIDialogType.帮助,
                windowtitle: "XHud - 元素生成器消息",
                title: "推送元素入库",
                msg: msg,
                ok: "立即推送",
                cancel: "暂不",
                PrimaryIndex: 0,
                usemodal: true,
                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                on_selected: (d) =>
                {
                    if (d == "立即推送")
                    {
                        EditorApplication.delayCall += () =>
                        {
                            // 这里调用你之前抽出来的公共方法
                            Editor_XHud_LibrarySetTool_ElementImporter.OpenWithElements(elements);
                        };
                    }
                });
        }
        /// <summary>
        /// 播放模式状态变化时的处理，退出编辑模式时清空待处理队列
        /// </summary>
        /// <param name="state">
        /// 当前播放模式状态
        /// </param>
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                // 退出编辑模式时，清理待处理队列
                pending_prefabs.Clear();
            }
        }
        #endregion

        #region 预制体处理
        /// <summary>
        /// 处理已就绪的预制体：加载预制体、输出日志并触发就绪事件
        /// </summary>
        /// <param name="assetPath">
        /// 预制体资源路径
        /// </param>
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
        /// 手动触发预制体处理（用于已有预制体或测试）
        /// </summary>
        /// <param name="assetPath">
        /// 预制体路径
        /// </param>
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
        #endregion
    }

    /// <summary>
    /// 内部辅助类：用于接入 AssetModificationProcessor
    /// </summary>
    internal class AssetModificationProcessorHook : UnityEditor.AssetModificationProcessor
    {
        #region 字段 - 事件
        /// <summary>
        /// 资源即将创建事件（参数为资源路径）
        /// </summary>
        public static event System.Action<string> on_will_create_asset_handler;
        #endregion

        #region 回调
        /// <summary>
        /// 资源即将创建时的回调，转发给 on_will_create_asset_handler
        /// </summary>
        /// <param name="assetName">
        /// 即将创建的资源名称
        /// </param>
        public static void OnWillCreateAsset(string assetName)
        {
            on_will_create_asset_handler?.Invoke(assetName);
        }
        #endregion
    }
}