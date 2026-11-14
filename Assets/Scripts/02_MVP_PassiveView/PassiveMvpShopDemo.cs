using System;
using UnityEngine;

namespace ArchitectureExamples.PassiveMvp
{
    /// <summary>对应图 03。OnEnable 中的 new 仅负责装配，实际展示逻辑在 Presenter。</summary>
    public sealed class PassiveMvpShopDemo : MonoBehaviour, IShopView
    {
        public event Action<ShopInput> Input;
        private ShopPresenter presenter;
        private string summary = "";
        private string buyText = "Buy";
        private bool canBuy;

        private void OnEnable() { presenter = new ShopPresenter(new ShopModel(), this); }
        private void OnDisable() { presenter?.Dispose(); }
        public void Render(ShopViewState state)
        {
            // View 只是照抄 Presenter 交给它的状态，不计算价格或可购买条件。
            summary = state.Summary; buyText = state.BuyText; canBuy = state.CanBuy;
        }
        // IMGUI 只用于免搭建场景的演示。多次 OnGUI 不代表业务状态被重复计算。
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(290, 12, Mathf.Max(280, Screen.width - 310), 650), GUI.skin.box);
            GUILayout.Label("02 | MVP: Passive View");
            GUILayout.Space(10);
            GUILayout.Label(summary);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Count -")) Input?.Invoke(ShopInput.Less);
            if (GUILayout.Button("Count +")) Input?.Invoke(ShopInput.More);
            if (GUILayout.Button("Price 100 / 150")) Input?.Invoke(ShopInput.TogglePrice);
            GUILayout.EndHorizontal();
            bool previousEnabled = GUI.enabled;
            GUI.enabled = previousEnabled && canBuy;
            if (GUILayout.Button(buyText)) Input?.Invoke(ShopInput.Buy);
            GUI.enabled = previousEnabled;
            if (GUILayout.Button("Change together: Price=150, Count=3")) Input?.Invoke(ShopInput.ChangeOffer);
            if (GUILayout.Button("Reset")) Input?.Invoke(ShopInput.Reset);

            GUILayout.EndArea();
        }
    }
}
