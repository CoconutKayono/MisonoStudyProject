using System;

namespace ArchitectureExamples.ClassicMvc
{
    /// <summary>输入解释器：把用户意图转换为模型操作，不负责推导按钮文案。</summary>
    public sealed class ShopController
    {
        private readonly ShopModel model;
        public ShopController(ShopModel model) { this.model = model; }
        public void Handle(ShopInput input)
        {
            switch (input)
            {
                case ShopInput.Less: model.SetCount(model.Count - 1); break;
                case ShopInput.More: model.SetCount(model.Count + 1); break;
                case ShopInput.TogglePrice: model.SetPrice(model.Price == 100 ? 150 : 100); break;
                case ShopInput.Buy: model.TryBuy(); break;
                case ShopInput.ChangeOffer: model.ChangeOffer(); break;
                case ShopInput.Reset: model.Reset(); break;
            }
        }
    }
    public enum ShopInput { Less, More, TogglePrice, Buy, ChangeOffer, Reset }
}
