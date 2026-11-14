using System;

namespace ArchitectureExamples.PassiveMvp
{
    public readonly struct ShopViewState
    {
        public readonly string Summary, BuyText;
        public readonly bool CanBuy;
        public ShopViewState(string summary, string buyText, bool canBuy)
        { Summary = summary; BuyText = buyText; CanBuy = canBuy; }
    }

    /// <summary>被动 View：只上报输入、接受完整展示状态。Presenter 可以配合假 View 测试。</summary>
    public interface IShopView
    {
        event Action<ShopInput> Input;
        void Render(ShopViewState state);
    }
    public enum ShopInput { Less, More, TogglePrice, Buy, ChangeOffer, Reset }
}
