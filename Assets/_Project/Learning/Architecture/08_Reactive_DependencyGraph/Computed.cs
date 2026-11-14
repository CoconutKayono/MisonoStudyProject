using System;

namespace ArchitectureExamples.DependencyGraph
{
    /// <summary>缓存派生值：失效时仅标记 Dirty，消费者读取时才重新计算。</summary>
    public sealed class Computed<T> : ReactiveObserver
    {
        private readonly Func<T> calculate;
        private T cached;
        private bool dirty = true;
        private bool computing;
        public int EvaluationCount { get; private set; }
        public bool IsDirty => dirty;
        public Computed(ReactiveRuntime runtime, Func<T> calculate) : base(runtime) { this.calculate = calculate; }
        public T Value
        {
            get
            {
                if (IsDisposed) throw new ObjectDisposedException(nameof(Computed<T>));
                Track(); // 外层 Effect/Computed 依赖当前节点，而不是直接依赖其全部源。
                if (!dirty) return cached;
                if (computing) throw new InvalidOperationException("Circular computed dependency.");
                computing = true;
                try
                {
                    cached = Evaluate(calculate);
                    EvaluationCount++;
                    dirty = false;
                    return cached;
                }
                finally { computing = false; }
            }
        }
        internal override void Invalidate()
        {
            if (dirty || IsDisposed) return;
            dirty = true;
            InvalidateObservers(); // Dirty 往下游推，数值暂不推。
        }
    }
}
