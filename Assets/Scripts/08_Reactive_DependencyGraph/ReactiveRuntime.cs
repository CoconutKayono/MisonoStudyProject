using System;
using System.Collections.Generic;

namespace ArchitectureExamples.DependencyGraph
{
    /// <summary>
    /// 单线程教学运行时：当前观察者负责自动收集依赖；队列 + 集合负责 Effect 去重。
    /// 仅用于 Unity 主线程，不处理异步调度、跨线程访问、完整 Rx 流协议或生产级循环恢复。
    /// </summary>
    public sealed class ReactiveRuntime
    {
        internal ReactiveObserver Current;
        private readonly Queue<ReactiveEffect> queue = new Queue<ReactiveEffect>();
        private readonly HashSet<ReactiveEffect> pending = new HashSet<ReactiveEffect>();
        private int batchDepth;
        private bool flushing;

        public void Batch(Action action)
        {
            batchDepth++;
            try { action(); }
            finally
            {
                batchDepth--;
                if (batchDepth == 0) Flush();
            }
        }
        internal void Schedule(ReactiveEffect effect)
        {
            if (!effect.IsDisposed && pending.Add(effect)) queue.Enqueue(effect);
        }
        private void Flush()
        {
            if (flushing) return;
            flushing = true;
            int iterations = 0;
            try
            {
                while (queue.Count > 0)
                {
                    if (++iterations > 1000) throw new InvalidOperationException("Reactive cycle: effects should only render, not write their own sources.");
                    var effect = queue.Dequeue();
                    pending.Remove(effect);
                    if (!effect.IsDisposed) effect.Run();
                }
            }
            finally { flushing = false; }
        }
    }
}
