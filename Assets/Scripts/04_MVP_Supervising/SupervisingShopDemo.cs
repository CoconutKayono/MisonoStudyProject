using System;
using UnityEngine;

namespace ArchitectureExamples.SupervisingMvp
{
    /// <summary>对应图 04 右：与 Passive View 相比，这里的 View 可以读取 Model 的简单数据。</summary>
    public sealed class SupervisingShopDemo : MonoBehaviour
    {
        public event Action<ShopInput> Input;
        private ShopModel model;
        private ShopPresenter presenter;
        private string summary = "";
        private string buyText = "Buy";
        private bool canBuy;

        private void OnEnable()
        {
            model = new ShopModel();
            model.Changed += RefreshSimpleFields;
            presenter = new ShopPresenter(model, this);
            RefreshSimpleFields();
        }
        private void OnDisable()
        {
            presenter?.Dispose();
            if (model != null) model.Changed -= RefreshSimpleFields;
        }
        private void RefreshSimpleFields()
        {
            // 简单绑定路径：Model -> View。没有经过 Presenter.Render。
            summary = $"Price: {model.Price}    Count: {model.Count}\nWallet: {model.Wallet}    Owned: {model.Owned}\nTotal: {model.Total}";
        }
        public void ShowPurchase(string text, bool enabled)
        { buyText = text; canBuy = enabled; }
        // IMGUI 只用于免搭建场景的演示。多次 OnGUI 不代表业务状态被重复计算。
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(290, 12, Mathf.Max(280, Screen.width - 310), 650), GUI.skin.box);
            GUILayout.Label("04 | MVP: Supervising Controller");
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
