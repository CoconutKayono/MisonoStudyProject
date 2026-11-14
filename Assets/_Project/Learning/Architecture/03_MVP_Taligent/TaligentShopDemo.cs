using System;
using UnityEngine;

namespace ArchitectureExamples.TaligentMvp
{
    /// <summary>对应图 04 左：View、Presenter、Interactor、Command、Selection、Model。</summary>
    public sealed class TaligentShopDemo : MonoBehaviour
    {
        private ShopModel model;
        private ShopSelection selection;
        private ShopInteractor interactor;
        private ShopPresenter presenter;
        private string summary = "";
        private string buyText = "Buy";
        private bool canBuy;

        private void OnEnable()
        {
            model = new ShopModel(); selection = new ShopSelection(); interactor = new ShopInteractor();
            selection.Select(model);
            presenter = new ShopPresenter(model, selection, interactor, this);
        }
        private void OnDisable() { presenter?.Dispose(); }
        public void Render(string text, string buttonText, bool enabled)
        { summary = text; buyText = buttonText; canBuy = enabled; }
        // IMGUI 只用于免搭建场景的演示。多次 OnGUI 不代表业务状态被重复计算。
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(290, 12, Mathf.Max(280, Screen.width - 310), 650), GUI.skin.box);
            GUILayout.Label("03 | Taligent: six cooperating roles");
            GUILayout.Space(10);
            GUILayout.Label(summary);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Count -")) interactor.Interpret(ShopGesture.Less);
            if (GUILayout.Button("Count +")) interactor.Interpret(ShopGesture.More);
            if (GUILayout.Button("Price 100 / 150")) interactor.Interpret(ShopGesture.TogglePrice);
            GUILayout.EndHorizontal();
            bool previousEnabled = GUI.enabled;
            GUI.enabled = previousEnabled && canBuy;
            if (GUILayout.Button(buyText)) interactor.Interpret(ShopGesture.Buy);
            GUI.enabled = previousEnabled;
            if (GUILayout.Button("Change together: Price=150, Count=3")) interactor.Interpret(ShopGesture.ChangeOffer);
            if (GUILayout.Button("Reset")) interactor.Interpret(ShopGesture.Reset);
            GUILayout.Label(selection.Target == null ? "Selection: none (commands do nothing)" : "Selection: current product");
            if (GUILayout.Button("Select / clear product")) selection.Select(selection.Target == null ? model : null);
            GUILayout.EndArea();
        }
    }
}
