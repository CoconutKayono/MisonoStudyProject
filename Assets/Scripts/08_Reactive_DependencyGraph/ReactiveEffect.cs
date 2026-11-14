using System;

namespace ArchitectureExamples.DependencyGraph
{
    /// <summary>Effect 是副作用终点：首次执行收集依赖，失效后排队，Batch 结束时执行。</summary>
    public sealed class ReactiveEffect : ReactiveObserver
    {
        private readonly Action render;
        public int RunCount { get; private set; }
        public ReactiveEffect(ReactiveRuntime runtime, Action render) : base(runtime)
        { this.render = render; Run(); }
        internal void Run()
        {
            Evaluate(() => { render(); return true; });
            RunCount++;
        }
        internal override void Invalidate() { Runtime.Schedule(this); }
    }
}
