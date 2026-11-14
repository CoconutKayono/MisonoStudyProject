using System;
using UnityEngine;

namespace ArchitectureExamples.ClassicMvc
{
    /// <summary>
    /// View + 装配入口。对应图 01：输入 -> Controller -> Model；Changed -> View 查询 Model。
    /// 只模拟经典 MVC 的协作关系，不复刻 Smalltalk 的窗口系统或输入设备调度。
    /// </summary>
    public sealed class MvcShopDemo : MonoBehaviour
    {
        private ShopModel model;
        private ShopController controller;
        private string summary = "";
        private string buyText = "Buy";
        private bool canBuy;

        private void OnEnable()
        {
            model = new ShopModel();
            controller = new ShopController(model);
            model.Changed += RefreshFromModel; // View 直接观察 Model。
            RefreshFromModel();               // 初始状态也需要呈现一次。
        }
        private void OnDisable() { if (model != null) model.Changed -= RefreshFromModel; }

        private void RefreshFromModel()
        {
            // 收到的是“变化通知”，数据由 View 主动拉取；展示判断留在 View。
            summary = $"Price: {model.Price}    Count: {model.Count}\nWallet: {model.Wallet}    Owned: {model.Owned}\nTotal: {model.Total}";
            canBuy = model.CanBuy;
            buyText = canBuy ? $"Buy ({model.Total})" : "Not enough money";
        }
        // IMGUI 只用于免搭建场景的演示。多次 OnGUI 不代表业务状态被重复计算。
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(290, 12, Mathf.Max(280, Screen.width - 310), 650), GUI.skin.box);
            GUILayout.Label("01 | Classic MVC");
            GUILayout.Space(10);
            GUILayout.Label(summary);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Count -")) controller.Handle(ShopInput.Less);
            if (GUILayout.Button("Count +")) controller.Handle(ShopInput.More);
            if (GUILayout.Button("Price 100 / 150")) controller.Handle(ShopInput.TogglePrice);
            GUILayout.EndHorizontal();
            bool previousEnabled = GUI.enabled;
            GUI.enabled = previousEnabled && canBuy;
            if (GUILayout.Button(buyText)) controller.Handle(ShopInput.Buy);
            GUI.enabled = previousEnabled;
            if (GUILayout.Button("Change together: Price=150, Count=3")) controller.Handle(ShopInput.ChangeOffer);
            if (GUILayout.Button("Reset")) controller.Handle(ShopInput.Reset);

            GUILayout.EndArea();
        }
    }
}
