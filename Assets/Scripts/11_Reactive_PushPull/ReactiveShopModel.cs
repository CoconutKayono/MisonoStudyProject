using System;
using ArchitectureExamples.DependencyGraph;

namespace ArchitectureExamples.HybridReactive
{
    /// <summary>
    /// 源是 Price/Count/Wallet/Owned；Total/CanBuy 是派生节点，购买规则仍封装在模型里。
    /// 这里不写 Price.Changed += ...；依赖由计算函数读取 Value 时自动发现。
    /// </summary>
    public sealed class ReactiveShopModel : IDisposable
    {
        public ReactiveRuntime Runtime { get; } = new ReactiveRuntime();
        public ReactiveProperty<int> Price { get; }
        public ReactiveProperty<int> Count { get; }
        public ReactiveProperty<int> Wallet { get; }
        public ReactiveProperty<int> Owned { get; }
        public Computed<int> Total { get; }
        public Computed<bool> CanBuy { get; }
        public ReactiveShopModel()
        {
            Price = new ReactiveProperty<int>(Runtime, 100);
            Count = new ReactiveProperty<int>(Runtime, 2);
            Wallet = new ReactiveProperty<int>(Runtime, 500);
            Owned = new ReactiveProperty<int>(Runtime, 0);
            Total = new Computed<int>(Runtime, () => Price.Value * Count.Value);
            CanBuy = new Computed<bool>(Runtime, () => Wallet.Value >= Total.Value);
        }
        public void Less() { Count.Value = Math.Max(1, Count.Value - 1); }
        public void More() { Count.Value = Math.Min(5, Count.Value + 1); }
        public void TogglePrice() { Price.Value = Price.Value == 100 ? 150 : 100; }
        public bool TryBuy()
        {
            if (!CanBuy.Value) return false;
            Runtime.Batch(() => { Wallet.Value -= Total.Value; Owned.Value += Count.Value; });
            return true;
        }
        public void ChangeOffer()
        {
            // 先依次推送 Dirty，Effect 入队去重；退出 Batch 后才读取最终的 150 * 3。
            Runtime.Batch(() => { Price.Value = 150; Count.Value = 3; });
        }
        public void Reset()
        {
            Runtime.Batch(() => { Price.Value = 100; Count.Value = 2; Wallet.Value = 500; Owned.Value = 0; });
        }
        public void Dispose() { CanBuy.Dispose(); Total.Dispose(); }
    }
}
