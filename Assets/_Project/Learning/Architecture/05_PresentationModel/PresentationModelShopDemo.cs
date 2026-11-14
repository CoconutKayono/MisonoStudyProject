using System;
using UnityEngine;

namespace ArchitectureExamples.Presentation
{
    /// <summary>对应图 06 中列。View 仅呈现展示模型提供的结果。</summary>
    public sealed class PresentationModelShopDemo : MonoBehaviour
    {
        private ShopPresentationModel presentation;
        private string summary = "";
        private string buyText = "Buy";
        private bool canBuy;

        private void OnEnable() { presentation = new ShopPresentationModel(new ShopModel()); Refresh(); }
        // 显式轮询，是本示例选择的同步机制；不是 Presentation Model 的定义要求。
        private void Update() { Refresh(); }
        private void Refresh()
        { summary = presentation.Summary; buyText = presentation.BuyText; canBuy = presentation.CanBuy; }
        // IMGUI 只用于免搭建场景的演示。多次 OnGUI 不代表业务状态被重复计算。
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(290, 12, Mathf.Max(280, Screen.width - 310), 650), GUI.skin.box);
            GUILayout.Label("05 | Presentation Model (polling)");
            GUILayout.Space(10);
            GUILayout.Label(summary);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Count -")) presentation.Handle(ShopInput.Less);
            if (GUILayout.Button("Count +")) presentation.Handle(ShopInput.More);
            if (GUILayout.Button("Price 100 / 150")) presentation.Handle(ShopInput.TogglePrice);
            GUILayout.EndHorizontal();
            bool previousEnabled = GUI.enabled;
            GUI.enabled = previousEnabled && canBuy;
            if (GUILayout.Button(buyText)) presentation.Handle(ShopInput.Buy);
            GUI.enabled = previousEnabled;
            if (GUILayout.Button("Change together: Price=150, Count=3")) presentation.Handle(ShopInput.ChangeOffer);
            if (GUILayout.Button("Reset")) presentation.Handle(ShopInput.Reset);

            GUILayout.EndArea();
        }
    }
}
