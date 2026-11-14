using System;

namespace ArchitectureExamples.AutoMvvm
{
    /// <summary>
    /// 领域模型：保存业务数据、维护规则，不引用 View、Presenter、控件或 Unity API。
    /// 所有例子规则一致：单价 100/150，数量 1..5，余额初始 500，购买时扣款并增加已拥有数量。
    /// Changed 只表示“发生变化”；订阅者需要自行读取最新状态。
    /// </summary>
    public sealed class ShopModel
    {
        public int Price { get; private set; } = 100;
        public int Count { get; private set; } = 2;
        public int Wallet { get; private set; } = 500;
        public int Owned { get; private set; }
        public int Total => Price * Count;
        public bool CanBuy => Wallet >= Total;
        public event Action Changed;

        public void SetCount(int value)
        {
            // 按钮是否可点只是展示；模型仍要独立保护业务边界。
            value = Math.Max(1, Math.Min(5, value));
            if (Count == value) return;
            Count = value;
            Changed?.Invoke();
        }

        public void SetPrice(int value)
        {
            if (value != 100 && value != 150) throw new ArgumentOutOfRangeException(nameof(value));
            if (Price == value) return;
            Price = value;
            Changed?.Invoke();
        }

        public bool TryBuy()
        {
            if (!CanBuy) return false;
            Wallet -= Total;
            Owned += Count;
            // 一次业务操作修改多个字段后统一通知，观察者不会看到“只扣款，未发货”。
            Changed?.Invoke();
            return true;
        }

        public void ChangeOffer()
        {
            if (Price == 150 && Count == 3) return;
            Price = 150;
            Count = 3;
            Changed?.Invoke();
        }

        public void Reset()
        {
            Price = 100; Count = 2; Wallet = 500; Owned = 0;
            Changed?.Invoke();
        }
    }
}
