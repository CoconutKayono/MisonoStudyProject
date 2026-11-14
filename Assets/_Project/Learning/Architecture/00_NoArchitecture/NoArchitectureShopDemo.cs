using System;
using UnityEngine;

namespace ArchitectureExamples.NoArchitecture
{
    /// <summary>基线：没有人为分出 Model、View 或控制层，方便观察后面把哪些职责搬走。</summary>
    public sealed class NoArchitectureShopDemo : MonoBehaviour
    {
        private int price, count, wallet, owned;
        private string summary = "";
        private string buyText = "Buy";
        private bool canBuy;

        private void OnEnable() { ResetData(); Refresh(); }

        private void ResetData() { price = 100; count = 2; wallet = 500; owned = 0; }

        private void Refresh()
        {
            // 业务规则、展示状态、字符串格式化全部混在同一个脚本里。
            int total = price * count;
            summary = $"Price: {price}    Count: {count}\nWallet: {wallet}    Owned: {owned}\nTotal: {total}";
            canBuy = wallet >= total;
            buyText = canBuy ? $"Buy ({total})" : "Not enough money";
        }

        private void Handle(ShopInput input)
        {
            switch (input)
            {
                case ShopInput.Less: count = Mathf.Max(1, count - 1); break;
                case ShopInput.More: count = Mathf.Min(5, count + 1); break;
                case ShopInput.TogglePrice: price = price == 100 ? 150 : 100; break;
                case ShopInput.ChangeOffer: price = 150; count = 3; break;
                case ShopInput.Reset: ResetData(); break;
                case ShopInput.Buy:
                    // 即使 UI 已禁用按钮，真正修改数据前也再次验证余额。
                    if (wallet >= price * count) { wallet -= price * count; owned += count; }
                    break;
            }
            // 每个入口都要记得刷新。将来入口增多时，容易忘记调用。
            Refresh();
        }
        // IMGUI 只用于免搭建场景的演示。多次 OnGUI 不代表业务状态被重复计算。
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(290, 12, Mathf.Max(280, Screen.width - 310), 650), GUI.skin.box);
            GUILayout.Label("00 | No architecture");
            GUILayout.Space(10);
            GUILayout.Label(summary);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Count -")) Handle(ShopInput.Less);
            if (GUILayout.Button("Count +")) Handle(ShopInput.More);
            if (GUILayout.Button("Price 100 / 150")) Handle(ShopInput.TogglePrice);
            GUILayout.EndHorizontal();
            bool previousEnabled = GUI.enabled;
            GUI.enabled = previousEnabled && canBuy;
            if (GUILayout.Button(buyText)) Handle(ShopInput.Buy);
            GUI.enabled = previousEnabled;
            if (GUILayout.Button("Change together: Price=150, Count=3")) Handle(ShopInput.ChangeOffer);
            if (GUILayout.Button("Reset")) Handle(ShopInput.Reset);

            GUILayout.EndArea();
        }
    }
    public enum ShopInput { Less, More, TogglePrice, Buy, ChangeOffer, Reset }
}
