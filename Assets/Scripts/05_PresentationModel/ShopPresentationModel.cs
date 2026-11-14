using System;

namespace ArchitectureExamples.Presentation
{
    /// <summary>
    /// 展示模型抽离文案和按钮状态，不认识 View。与下一节对比：这里暂不提供 PropertyChanged/Command。
    /// Presentation Model 与 MVVM 思想相关，但不是对所有框架都成立的单线历史继承关系。
    /// </summary>
    public sealed class ShopPresentationModel
    {
        private readonly ShopModel model;
        public ShopPresentationModel(ShopModel model) { this.model = model; }
        public string Summary => $"Price: {model.Price}    Count: {model.Count}\nWallet: {model.Wallet}    Owned: {model.Owned}\nTotal: {model.Total}";
        public string BuyText => model.CanBuy ? $"Buy ({model.Total})" : "Not enough money";
        public bool CanBuy => model.CanBuy;
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
