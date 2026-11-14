using System.Collections.Generic;
using UnityEngine;
using ArchitectureExamples.DependencyGraph;

namespace ArchitectureExamples.DependencyGraph
{
    /// <summary>对应参考图 07：自动依赖追踪、Dirty、Batch、Effect。</summary>
    public sealed class DependencyGraphShopDemo : MonoBehaviour
    {
        private ReactiveShopModel model;
        private ReactiveEffect summaryEffect, totalEffect, buttonEffect;
        private readonly List<int> totals = new List<int>();
        private int lastTotal;
        private string summary = "";
        private string buyText = "Buy";
        private bool canBuy;

        private void OnEnable()
        {
            totals.Clear();
            model = new ReactiveShopModel();
            // 三个 Effect 有重叠依赖。比如 Price -> Total -> CanBuy -> 按钮 Effect。
            summaryEffect = new ReactiveEffect(model.Runtime, () =>
                summary = $"Price: {model.Price.Value}    Count: {model.Count.Value}\nWallet: {model.Wallet.Value}    Owned: {model.Owned.Value}");
            totalEffect = new ReactiveEffect(model.Runtime, () =>
            {
                lastTotal = model.Total.Value;
                totals.Add(lastTotal);
            });
            buttonEffect = new ReactiveEffect(model.Runtime, () =>
            {
                canBuy = model.CanBuy.Value;
                buyText = canBuy ? $"Buy ({model.Total.Value})" : "Not enough money";
            });
        }
        private void OnDisable()
        {
            // 先解除副作用订阅，再释放派生节点。反复切换不累积观察者。
            summaryEffect?.Dispose(); totalEffect?.Dispose(); buttonEffect?.Dispose(); model?.Dispose();
        }
        private void Buy() { model.TryBuy(); }
        private void Reset()
        {
            totals.Clear();
            model.Reset();
            if (totals.Count == 0) totals.Add(model.Total.Value);
        }
        // 绘制缓存的 UI 状态；业务操作只发生在按钮点击时。
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(290, 12, Mathf.Max(280, Screen.width - 310), 650), GUI.skin.box);
            GUILayout.Label("08 | Reactive: dependency graph");
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
            GUILayout.Label($"Total: {lastTotal}");
            GUILayout.Label("Consumed Total: " + string.Join(" -> ", totals));
            GUILayout.Label($"Total calculations: {model.Total.EvaluationCount}; UI effect runs: {summaryEffect.RunCount}");
            GUILayout.Label("Batch: 200 -> 450 (no intermediate 300 is rendered)");
            GUILayout.EndArea();
        }
    }
}
