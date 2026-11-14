using System;
using System.ComponentModel;

namespace ArchitectureExamples.ManualMvvm
{
    /// <summary>手动绑定适配层：监听属性通知，再把 ViewModel 的值抄到 View。没有领域规则。</summary>
    public sealed class ShopBinding : IDisposable
    {
        private readonly ShopViewModel viewModel;
        private readonly ManualMvvmShopDemo view;
        public ShopBinding(ShopViewModel viewModel, ManualMvvmShopDemo view)
        {
            this.viewModel = viewModel; this.view = view;
            viewModel.PropertyChanged += OnPropertyChanged;
            Synchronize(); // 订阅变化不会自动收到初始值，所以先同步一次。
        }
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            // 教学版统一同步这三个小属性；大型 UI 可按 args.PropertyName 局部更新。
            Synchronize();
        }
        private void Synchronize() { view.Render(viewModel.Summary, viewModel.BuyText, viewModel.CanBuy); }
        public void Dispose() { viewModel.PropertyChanged -= OnPropertyChanged; }
    }
}
