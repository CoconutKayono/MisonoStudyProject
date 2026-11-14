using System;
using System.Collections.Generic;

namespace ArchitectureExamples.DependencyGraph
{
    /// <summary>依赖图的节点。边 Source -> Observer 表示 Observer 读取过 Source。</summary>
    public abstract class ReactiveNode
    {
        protected readonly ReactiveRuntime Runtime;
        private readonly HashSet<ReactiveObserver> observers = new HashSet<ReactiveObserver>();
        protected ReactiveNode(ReactiveRuntime runtime) { Runtime = runtime; }
        protected void Track() { Runtime.Current?.Observe(this); }
        internal void Add(ReactiveObserver observer) { observers.Add(observer); }
        internal void Remove(ReactiveObserver observer) { observers.Remove(observer); }
        protected void InvalidateObservers()
        {
            // 使用快照，允许订阅者在重新执行时解除旧依赖。
            foreach (var observer in new List<ReactiveObserver>(observers))
                if (!observer.IsDisposed) observer.Invalidate();
        }
    }

    /// <summary>Computed 和 Effect 都会读取其他节点，因此共享依赖收集与清理逻辑。</summary>
    public abstract class ReactiveObserver : ReactiveNode, IDisposable
    {
        private readonly HashSet<ReactiveNode> dependencies = new HashSet<ReactiveNode>();
        public bool IsDisposed { get; private set; }
        protected ReactiveObserver(ReactiveRuntime runtime) : base(runtime) { }
        internal void Observe(ReactiveNode source)
        {
            if (dependencies.Add(source)) source.Add(this);
        }
        protected T Evaluate<T>(Func<T> read)
        {
            if (IsDisposed) throw new ObjectDisposedException(GetType().Name);
            // 每次重新收集，才能处理 if/else 分支导致的动态依赖变化。
            Detach();
            var previous = Runtime.Current;
            Runtime.Current = this;
            try { return read(); }
            finally { Runtime.Current = previous; }
        }
        internal abstract void Invalidate();
        private void Detach()
        {
            foreach (var dependency in dependencies) dependency.Remove(this);
            dependencies.Clear();
        }
        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            Detach(); // 解除关系，而不是只把一个“已销毁”标记留在依赖图上。
        }
    }
}
