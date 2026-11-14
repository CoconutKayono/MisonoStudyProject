using System;
using UnityEngine;

namespace ArchitectureExamples.ManualMvvm
{
    /// <summary>对应图 05 右。这里用 IMGUI 代替 UGUI 控件，保留手写 PropertyChanged 绑定机制。</summary>
    public sealed class ManualMvvmShopDemo : MonoBehaviour
    {
        private ShopViewModel viewModel;
        private ShopBinding binding;
        private string summary = "";
        private string buyText = "Buy";
        private bool canBuy;

        private void OnEnable()
        {
            viewModel = new ShopViewModel(new ShopModel());
            binding = new ShopBinding(viewModel, this);
        }
        private void OnDisable() { binding?.Dispose(); viewModel?.Dispose(); }
        public void Render(string text, string buttonText, bool enabled)
        { summary = text; buyText = buttonText; canBuy = enabled; }
        // IMGUI 只用于免搭建场景的演示。多次 OnGUI 不代表业务状态被重复计算。
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(290, 12, Mathf.Max(280, Screen.width - 310), 650), GUI.skin.box);
            GUILayout.Label("06 | MVVM: manual PropertyChanged binding");
            GUILayout.Space(10);
            GUILayout.Label(summary);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Count -")) viewModel.LessCommand.Execute();
            if (GUILayout.Button("Count +")) viewModel.MoreCommand.Execute();
            if (GUILayout.Button("Price 100 / 150")) viewModel.TogglePriceCommand.Execute();
            GUILayout.EndHorizontal();
            bool previousEnabled = GUI.enabled;
            GUI.enabled = previousEnabled && canBuy;
            if (GUILayout.Button(buyText)) viewModel.BuyCommand.Execute();
            GUI.enabled = previousEnabled;
            if (GUILayout.Button("Change together: Price=150, Count=3")) viewModel.ChangeOfferCommand.Execute();
            if (GUILayout.Button("Reset")) viewModel.ResetCommand.Execute();

            GUILayout.EndArea();
        }
    }
}
