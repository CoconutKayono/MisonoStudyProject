using System;
using Unity.Properties;
using UnityEngine.UIElements;

namespace ArchitectureExamples.AutoMvvm
{
    /// <summary>
    /// ViewModel 暴露“展示属性 + 命令”，始终不持有 View、Button 或 UIDocument。
    /// Unity UI Toolkit 通过 CreateProperty 发现属性，通过 INotifyBindablePropertyChanged 接收通知。
    /// </summary>
    public sealed class ShopViewModel : INotifyBindablePropertyChanged, IDisposable
    {
        private readonly ShopModel model;
        [CreateProperty] public string Summary => $"Price: {model.Price}    Count: {model.Count}\nWallet: {model.Wallet}    Owned: {model.Owned}\nTotal: {model.Total}";
        [CreateProperty] public string BuyText => model.CanBuy ? $"Buy ({model.Total})" : "Not enough money";
        [CreateProperty] public bool CanBuy => model.CanBuy;
        public ShopCommand LessCommand { get; }
        public ShopCommand MoreCommand { get; }
        public ShopCommand TogglePriceCommand { get; }
        public ShopCommand BuyCommand { get; }
        public ShopCommand ChangeOfferCommand { get; }
        public ShopCommand ResetCommand { get; }
        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;
        private void Notify(string name)
        {
            BindingId id = name;
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(id));
        }
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
