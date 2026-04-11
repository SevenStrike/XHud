namespace SevenStrikeModules.XHud.Editor
{
    /// <summary>
    /// 编辑器协程等待秒数指令。暂停编辑器协程执行指定的秒数（使用未缩放时间）。
    /// 协程将在指定时间过后继续执行。
    /// </summary>
    public class XCoroutineWaitForSeconds
    {
        /// <summary>
        /// 等待的时间（秒）。
        /// </summary>
        public float WaitTime { get; }

        /// <summary>
        /// 创建一个等待秒数指令对象，用于在协程函数中 yield 返回。
        /// </summary>
        /// <param name="time">等待的时间（秒）</param>
        public XCoroutineWaitForSeconds(float time)
        {
            WaitTime = time;
        }
    }
}