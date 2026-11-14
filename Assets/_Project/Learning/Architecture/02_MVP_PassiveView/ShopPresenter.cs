using System;

namespace ArchitectureExamples.PassiveMvp
{
    /// <summary>Presenter 观察模型，并主动把全部展示状态推给 View。</summary>
    public sealed class ShopPresenter : IDisposable
    {
        private readonly ShopModel model;
        private readonly IShopView view;
        public ShopPresenter(ShopModel model, IShopView view)
        {
            this.model = model; this.view = view;
            view.Input += Handle;
            model.Changed += Present;
            Present();
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
        private void Present()
        {
            // 与 MVC 的关键区别：这些展示判断位于 Presenter，View 不再读取 Model。
            view.Render(new ShopViewState($"Price: {model.Price}    Count: {model.Count}\nWallet: {model.Wallet}    Owned: {model.Owned}\nTotal: {model.Total}",
                model.CanBuy ? $"Buy ({model.Total})" : "Not enough money", model.CanBuy));
        }
        public void Dispose() { view.Input -= Handle; model.Changed -= Present; }
    }
}
