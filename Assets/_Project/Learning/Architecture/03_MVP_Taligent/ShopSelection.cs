using System;

namespace ArchitectureExamples.TaligentMvp
{
    /// <summary>Selection 表达操作对象；取消选择不会删除商品或改变其业务数据。</summary>
    public sealed class ShopSelection
    {
        public ShopModel Target { get; private set; }
        public event Action Changed;
        public void Select(ShopModel target) { Target = target; Changed?.Invoke(); }
    }
}
