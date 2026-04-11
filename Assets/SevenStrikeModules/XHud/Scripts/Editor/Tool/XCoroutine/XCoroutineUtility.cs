namespace SevenStrikeModules.XHud.Editor
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// 编辑器协程工具类，提供启动和停止协程的静态方法。
    /// </summary>
    public static class XCoroutineUtility
    {
        /// <summary>
        /// 启动一个带所有者的编辑器协程。如果所有者对象被垃圾回收，协程将自动停止。
        /// </summary>
        /// <param name="routine">要执行的协程枚举器</param>
        /// <param name="owner">协程的所有者对象</param>
        /// <returns>编辑器协程句柄</returns>
        public static XCoroutine xec_StartCoroutine(IEnumerator routine, object owner)
        {
            if (routine == null)
            {
                Debug.LogError("无法启动空的协程。");
                return null;
            }
            return new XCoroutine(routine, owner);
        }

        /// <summary>
        /// 启动一个无所有者的编辑器协程。协程将一直运行直到完成或手动停止。
        /// </summary>
        /// <param name="routine">要执行的协程枚举器</param>
        /// <returns>编辑器协程句柄</returns>
        public static XCoroutine xec_StartCoroutineOwnerless(IEnumerator routine)
        {
            if (routine == null)
            {
                Debug.LogError("无法启动空的协程。");
                return null;
            }
            return new XCoroutine(routine);
        }

        /// <summary>
        /// 立即停止指定的编辑器协程。对已完成的协程调用此方法是安全的。
        /// </summary>
        /// <param name="coroutine">要停止的编辑器协程句柄</param>
        public static void xec_StopCoroutine(XCoroutine coroutine)
        {
            if (coroutine == null)
            {
                Debug.LogWarning("尝试停止空的编辑器协程句柄。");
                return;
            }
            coroutine.xec_Stop();
        }
    }
}