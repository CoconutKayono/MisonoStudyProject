using System.Collections.Generic;

namespace ArchitectureExamples.DependencyGraph
{
    /// <summary>可写源：getter Track，setter 修改并传播失效。</summary>
    public sealed class ReactiveProperty<T> : ReactiveNode
    {
        private T value;
        public ReactiveProperty(ReactiveRuntime runtime, T initial) : base(runtime) { value = initial; }
        public T Value
        {
            get { Track(); return value; }
            set
            {
                if (EqualityComparer<T>.Default.Equals(this.value, value)) return;
                Runtime.Batch(() =>
                {
                    this.value = value;
                    // 先传播完整个失效图，再 Flush Effect，避免菱形图读到半新半旧的缓存。
                    InvalidateObservers();
                });
            }
        }
    }
}
