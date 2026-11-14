using System;
using System.Collections.Generic;

namespace ArchitectureExamples.PullReactive
{
    /// <summary>
    /// Pull：源写入后仅标记 Dirty；读取 Total/CanBuy 才计算并缓存。
    /// 本节依赖明确写在代码中，便于单独比较调度时机；自动依赖图见 08。
    /// </summary>
    public sealed class ReactiveShopModel
    {
        public int Price { get; private set; } = 100;
        public int Count { get; private set; } = 2;
        public int Wallet { get; private set; } = 500;
        public int Owned { get; private set; }
        private int total;
        private bool canBuy;
        public bool IsDirty { get; private set; } = true;
        public int EvaluationCount { get; private set; }
        public readonly List<int> TotalHistory = new List<int>();
        // Pull 没有主动驱动 UI 的 Effect；由消费者决定何时读取。
        public int Total { get { EnsureFresh(); return total; } }
        public bool CanBuy { get { EnsureFresh(); return canBuy; } }
        public ReactiveShopModel() {  }
        private void Recalculate()
        {
            total = Price * Count;
            canBuy = Wallet >= total;
            IsDirty = false;
            EvaluationCount++;
            TotalHistory.Add(total);
        }
        private void EnsureFresh() { if (IsDirty) Recalculate(); }
        private void Invalidate()
        {
            IsDirty = true;
            // 故意不计算，不触发 UI；多次失效仍然只是 Dirty=true。
        }
        public void Less() { SetCount(Count - 1); }
        public void More() { SetCount(Count + 1); }
        private void SetCount(int value)
        {
            value = Math.Max(1, Math.Min(5, value));
            if (Count == value) return;
            Count = value; Invalidate();
        }
        private void SetPrice(int value)
        {
            if (Price == value) return;
            Price = value; Invalidate();
        }
        public void TogglePrice() { SetPrice(Price == 100 ? 150 : 100); }
        public void ChangeOffer()
        {
            // 与 11 对比：这里不使用 Batch，依次修改两个源。
            SetPrice(150); SetCount(3);
        }
        public bool TryBuy()
        {
            // Pull 即使界面快照尚未更新，这次读取也会验证最新业务状态。
            if (!CanBuy) return false;
            Wallet -= Total; Owned += Count;
            Invalidate();
            return true;
        }
        public void Reset()
        {
            Price = 100; Count = 2; Wallet = 500; Owned = 0;
            TotalHistory.Clear();
            Invalidate();
        }
    }
}
