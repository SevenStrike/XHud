namespace SevenStrikeModules.XHud.Editor
{
    using System.Collections;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// EditorWindow 的编辑器协程扩展方法。
    /// </summary>
    public static class XCoroutineExtension
    {
        /// <summary>
        /// 启动一个由调用方 EditorWindow 实例所有的编辑器协程。
        /// 当窗口关闭时，协程会自动停止。
        /// </summary>
        /// <param name="window">EditorWindow 实例</param>
        /// <param name="routine">要执行的协程枚举器</param>
        /// <returns>编辑器协程句柄</returns>
        public static XCoroutine xec_StartCoroutine(this EditorWindow window, IEnumerator routine)
        {
            if (window == null)
            {
                Debug.LogError("无法在空的 EditorWindow 上启动协程。");
                return null;
            }
            return new XCoroutine(routine, window);
        }

        /// <summary>
        /// 立即停止由调用方 EditorWindow 启动的编辑器协程。
        /// 对已完成的协程调用此方法是安全的。
        /// </summary>
        /// <param name="window">EditorWindow 实例</param>
        /// <param name="coroutine">要停止的编辑器协程句柄</param>
        public static void xec_StopCoroutine(this EditorWindow window, XCoroutine coroutine)
        {
            if (coroutine == null)
            {
                Debug.LogWarning("提供的编辑器协程句柄为空。");
                return;
            }

            // 无所有者或所有者已死亡，直接使用工具类停止
            if (coroutine.ownerRef == null || !coroutine.ownerRef.IsAlive)
            {
                XCoroutineUtility.xec_StopCoroutine(coroutine);
                return;
            }

            // 验证所有者是否是当前窗口
            var owner = coroutine.ownerRef.Target as EditorWindow;
            if (owner == null || owner != window)
            {
                Debug.LogWarningFormat("该编辑器协程被其他对象所有：{0}，已强制停止。", coroutine.ownerRef.Target);
                XCoroutineUtility.xec_StopCoroutine(coroutine);
                return;
            }

            XCoroutineUtility.xec_StopCoroutine(coroutine);
        }
    }
}