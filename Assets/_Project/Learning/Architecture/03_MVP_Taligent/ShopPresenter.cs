using System;

namespace ArchitectureExamples.TaligentMvp
{
    /// <summary>教学版六角色协作，不声称完整复刻 Taligent 的框架、撤销栈或选择范围。</summary>
    public sealed class ShopPresenter : IDisposable
    {
        private readonly ShopModel model;
        private readonly ShopSelection selection;
        private readonly ShopInteractor interactor;
        private readonly TaligentShopDemo view;
        public ShopPresenter(ShopModel model, ShopSelection selection, ShopInteractor interactor, TaligentShopDemo view)
        {
            this.model = model; this.selection = selection; this.interactor = interactor; this.view = view;
            interactor.CommandRequested += Execute;
            model.Changed += Present;
            selection.Changed += Present;
            Present();
        }
        private void Execute(ShopCommand command) { command.Execute(selection); }
        private void Present()
        {
            view.Render($"Price: {model.Price}    Count: {model.Count}\nWallet: {model.Wallet}    Owned: {model.Owned}\nTotal: {model.Total}",
                model.CanBuy ? $"Buy ({model.Total})" : "Not enough money",
                selection.Target != null && model.CanBuy);
        }
        public void Dispose()
        {
            interactor.CommandRequested -= Execute;
            model.Changed -= Present;
            selection.Changed -= Present;
        }
    }
}
