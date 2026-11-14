using System;
using UnityEngine;

namespace ArchitectureExamples.PushReactive
{
    /// <summary>对应图 08 左：立即推送。计算轨迹可直接在 Game 窗口中观察。</summary>
    public sealed class PushShopDemo : MonoBehaviour
    {
        private ReactiveShopModel model;
        private string summary = "";
        private string buyText = "Buy";
        private bool canBuy;

        private void OnEnable()
        {
            model = new ReactiveShopModel();
            model.Changed += ReadSnapshot;
            ReadSnapshot();
        }
        private void OnDisable() { if (model != null) model.Changed -= ReadSnapshot; }
        private void ReadSnapshot()
        {
            summary = $"Price: {model.Price}    Count: {model.Count}\nWallet: {model.Wallet}    Owned: {model.Owned}\nTotal: {model.Total}";
            canBuy = model.CanBuy;
            buyText = canBuy ? $"Buy ({model.Total})" : "Not enough money";
        }
        private void Buy() { model.TryBuy();  }
        private void Reset() { model.Reset();  }
        // 绘制缓存的 UI 状态；业务操作只发生在按钮点击时。
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(290, 12, Mathf.Max(280, Screen.width - 310), 650), GUI.skin.box);
            GUILayout.Label("09 | Reactive: immediate push");
            GUILayout.Space(10);
            GUILayout.Label(summary);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Count -")) model.Less();
            if (GUILayout.Button("Count +")) model.More();
            if (GUILayout.Button("Price 100 / 150")) model.TogglePrice();
            GUILayout.EndHorizontal();
            bool previousEnabled = GUI.enabled;
            GUI.enabled = previousEnabled && canBuy;
            if (GUILayout.Button(buyText)) Buy();
            GUI.enabled = previousEnabled;
            if (GUILayout.Button("Change together: Price=150, Count=3")) model.ChangeOffer();
            if (GUILayout.Button("Reset")) Reset();
            GUILayout.Label("Push: 200 -> 300 -> 450 (two immediate updates)");
            GUILayout.Label($"Current sources: Price={model.Price}, Count={model.Count}, Wallet={model.Wallet}, Owned={model.Owned}");
            GUILayout.Label($"Dirty: {model.IsDirty}; calculations: {model.EvaluationCount}");
            GUILayout.Label("Calculated Total: " + string.Join(" -> ", model.TotalHistory));
            
            GUILayout.EndArea();
        }
    }
}
