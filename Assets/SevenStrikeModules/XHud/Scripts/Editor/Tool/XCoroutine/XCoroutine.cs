namespace SevenStrikeModules.XHud.Editor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// 编辑器协程句柄，可传递给 <see cref="XCoroutineUtility">XCoroutineUtility</see> 的方法来控制生命周期。
    /// </summary>
    public class XCoroutine
    {
        /// <summary>
        /// 等待指令类型枚举。
        /// </summary>
        private enum WaitType
        {
            /// <summary>无等待</summary>
            None,
            /// <summary>等待指定秒数</summary>
            TimeDelay,
            /// <summary>等待子协程</summary>
            ChildCoroutine,
            /// <summary>等待异步操作</summary>
            AsyncOperation,
        }

        /// <summary>
        /// 协程的所有者弱引用。
        /// </summary>
        internal WeakReference ownerRef;

        /// <summary>
        /// 协程的主枚举器。
        /// </summary>
        private IEnumerator mainEnumerator;

        /// <summary>
        /// 协程是否已完成。
        /// </summary>
        private bool isCompleted;

        /// <summary>
        /// 获取协程是否已完成（供外部访问）。
        /// </summary>
        internal bool IsCompleted => isCompleted;

        /// <summary>
        /// 当前等待类型。
        /// </summary>
        private WaitType currentWaitType = WaitType.None;

        /// <summary>
        /// 等待结束时间（用于 TimeDelay）。
        /// </summary>
        private double waitEndTime;

        /// <summary>
        /// 等待的子协程（用于 ChildCoroutine）。
        /// </summary>
        private XCoroutine childCoroutine;

        /// <summary>
        /// 等待的异步操作（用于 AsyncOperation）。
        /// </summary>
        private AsyncOperation pendingAsyncOp;

        /// <summary>
        /// 用于嵌套枚举器处理的临时堆栈。
        /// </summary>
        private static readonly Stack<IEnumerator> enumeratorStack = new Stack<IEnumerator>(32);

        /// <summary>
        /// 构造函数 - 创建无所有者的协程。
        /// </summary>
        /// <param name="routine">协程枚举器</param>
        internal XCoroutine(IEnumerator routine)
        {
            ownerRef = null;
            mainEnumerator = routine;
            EditorApplication.update += OnEditorUpdate;
        }

        /// <summary>
        /// 构造函数 - 创建有所属者的协程。
        /// </summary>
        /// <param name="routine">协程枚举器</param>
        /// <param name="owner">协程所有者</param>
        internal XCoroutine(IEnumerator routine, object owner)
        {
            ownerRef = new WeakReference(owner);
            mainEnumerator = routine;
            EditorApplication.update += OnEditorUpdate;
        }

        /// <summary>
        /// 编辑器更新回调，驱动协程执行。
        /// </summary>
        private void OnEditorUpdate()
        {
            // 检查所有者是否仍然存活
            if (ownerRef != null && !ownerRef.IsAlive)
            {
                StopInternal();
                return;
            }

            // 处理当前等待条件
            if (!IsWaitConditionMet())
                return;

            // 推进主枚举器
            bool hasNext = ProcessEnumeratorChain(mainEnumerator);

            if (!hasNext)
            {
                isCompleted = true;
                StopInternal();
            }
        }

        /// <summary>
        /// 检查当前等待条件是否满足。
        /// </summary>
        /// <returns>条件满足返回 true，否则返回 false</returns>
        private bool IsWaitConditionMet()
        {
            switch (currentWaitType)
            {
                case WaitType.TimeDelay:
                    if (EditorApplication.timeSinceStartup >= waitEndTime)
                    {
                        currentWaitType = WaitType.None;
                        return true;
                    }
                    return false;

                case WaitType.ChildCoroutine:
                    // 修改这里：使用 childCoroutine.IsCompleted 而不是 childCoroutine.isCompleted
                    if (childCoroutine == null || childCoroutine.IsCompleted)
                    {
                        currentWaitType = WaitType.None;
                        childCoroutine = null;
                        return true;
                    }
                    return false;

                case WaitType.AsyncOperation:
                    if (pendingAsyncOp == null || pendingAsyncOp.isDone)
                    {
                        currentWaitType = WaitType.None;
                        pendingAsyncOp = null;
                        return true;
                    }
                    return false;

                default:
                    return true;
            }
        }

        /// <summary>
        /// 处理枚举器链，支持嵌套 IEnumerator。
        /// </summary>
        /// <param name="enumerator">起始枚举器</param>
        /// <returns>是否还有更多元素</returns>
        private bool ProcessEnumeratorChain(IEnumerator enumerator)
        {
            // 展平嵌套的枚举器链
            IEnumerator current = enumerator;
            while (current.Current is IEnumerator nested)
            {
                enumeratorStack.Push(current);
                current = nested;
            }

            // 处理当前等待指令
            ProcessYieldInstruction(current.Current);

            // 推进当前枚举器
            bool canContinue = current.MoveNext();

            // 回溯处理外层枚举器
            while (enumeratorStack.Count > 0 && !canContinue)
            {
                var parent = enumeratorStack.Pop();
                canContinue = parent.MoveNext();
            }

            enumeratorStack.Clear();
            return canContinue;
        }

        /// <summary>
        /// 处理 yield 返回的指令对象。
        /// </summary>
        /// <param name="instruction">yield 返回的对象</param>
        private void ProcessYieldInstruction(object instruction)
        {
            if (instruction == null)
            {
                currentWaitType = WaitType.None;
                return;
            }

            Type type = instruction.GetType();

            // 处理等待秒数
            if (type == typeof(XCoroutineWaitForSeconds))
            {
                var wait = instruction as XCoroutineWaitForSeconds;
                currentWaitType = WaitType.TimeDelay;
                waitEndTime = EditorApplication.timeSinceStartup + wait.WaitTime;
                childCoroutine = null;
                pendingAsyncOp = null;
            }
            // 处理子协程
            else if (type == typeof(XCoroutine))
            {
                currentWaitType = WaitType.ChildCoroutine;
                childCoroutine = instruction as XCoroutine;
                waitEndTime = -1;
                pendingAsyncOp = null;
            }
            // 处理异步操作
            else if (instruction is AsyncOperation op)
            {
                currentWaitType = WaitType.AsyncOperation;
                pendingAsyncOp = op;
                waitEndTime = -1;
                childCoroutine = null;
            }
            // 普通对象或无等待指令
            else
            {
                currentWaitType = WaitType.None;
            }
        }

        /// <summary>
        /// 停止协程执行。
        /// </summary>
        private void StopInternal()
        {
            ownerRef = null;
            mainEnumerator = null;
            childCoroutine = null;
            pendingAsyncOp = null;
            EditorApplication.update -= OnEditorUpdate;
        }

        /// <summary>
        /// 内部停止方法，供工具类调用。
        /// </summary>
        internal void xec_Stop()
        {
            StopInternal();
        }
    }
}