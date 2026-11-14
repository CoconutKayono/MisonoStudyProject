using System;
using System.ComponentModel;

namespace ArchitectureExamples.ManualMvvm
{
    /// <summary>
    /// ViewModel 暴露“展示属性 + 命令”，始终不持有 View、Button 或 UIDocument。
    /// 标准 INotifyPropertyChanged 只发送属性名，View/Binding 收到后读取属性。
    /// </summary>
    public sealed class ShopViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly ShopModel model;
        public string Summary => $"Price: {model.Price}    Count: {model.Count}\nWallet: {model.Wallet}    Owned: {model.Owned}\nTotal: {model.Total}";
        public string BuyText => model.CanBuy ? $"Buy ({model.Total})" : "Not enough money";
        public bool CanBuy => model.CanBuy;
        public ShopCommand LessCommand { get; }
        public ShopCommand MoreCommand { get; }
        public ShopCommand TogglePriceCommand { get; }
        public ShopCommand BuyCommand { get; }
        public ShopCommand ChangeOfferCommand { get; }
        public ShopCommand ResetCommand { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string name) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
        public ShopViewModel(ShopModel model)
        {
            this.model = model;
            LessCommand = new ShopCommand(() => model.SetCount(model.Count - 1));
            MoreCommand = new ShopCommand(() => model.SetCount(model.Count + 1));
            TogglePriceCommand = new ShopCommand(() => model.SetPrice(model.Price == 100 ? 150 : 100));
            BuyCommand = new ShopCommand(() => model.TryBuy(), () => model.CanBuy);
            ChangeOfferCommand = new ShopCommand(model.ChangeOffer);
            ResetCommand = new ShopCommand(model.Reset);
            model.Changed += OnModelChanged;
        }
        private void OnModelChanged()
        {
            // 这里显式列出依赖；与响应式 getter 自动 Track 的版本形成对照。
            Notify(nameof(Summary)); Notify(nameof(BuyText)); Notify(nameof(CanBuy));
        }
        public void Dispose() { model.Changed -= OnModelChanged; }
    }
}
