using System;

namespace ArchitectureExamples.SupervisingMvp
{
    /// <summary>只协调复合展示和输入；价格/数量等简单字段由 View 自己绑定。</summary>
    public sealed class ShopPresenter : IDisposable
    {
        private readonly ShopModel model;
        private readonly SupervisingShopDemo view;
        public ShopPresenter(ShopModel model, SupervisingShopDemo view)
        {
            this.model = model; this.view = view;
            model.Changed += PresentPurchase;
            view.Input += Handle;
            PresentPurchase();
        }
        private void Handle(ShopInput input)
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
        private void PresentPurchase()
        {
            // 示例的“复杂展示”是单价、数量、余额的组合文案；真正购买校验仍在 Model。
            view.ShowPurchase(model.CanBuy ? $"Buy ({model.Total})" : "Not enough money", model.CanBuy);
        }
        public void Dispose() { model.Changed -= PresentPurchase; view.Input -= Handle; }
    }
    public enum ShopInput { Less, More, TogglePrice, Buy, ChangeOffer, Reset }
}
