using System;

namespace ArchitectureExamples.TaligentMvp
{
    public enum ShopOperation { Less, More, TogglePrice, Buy, ChangeOffer, Reset }

    /// <summary>Command 封装“做什么”，目标来自 Selection；不持有 Unity 控件。</summary>
    public sealed class ShopCommand
    {
        private readonly ShopOperation operation;
        public ShopCommand(ShopOperation operation) { this.operation = operation; }
        public void Execute(ShopSelection selection)
        {
            ShopModel model = selection.Target;
            if (model == null) return; // 没有选择目标，命令不执行。
            switch (operation)
            {
                case ShopOperation.Less: model.SetCount(model.Count - 1); break;
                case ShopOperation.More: model.SetCount(model.Count + 1); break;
                case ShopOperation.TogglePrice: model.SetPrice(model.Price == 100 ? 150 : 100); break;
                case ShopOperation.Buy: model.TryBuy(); break;
                case ShopOperation.ChangeOffer: model.ChangeOffer(); break;
                case ShopOperation.Reset: model.Reset(); break;
            }
        }
    }
}
