using System;
using UnityEngine;

namespace ArchitectureExamples.NoArchitecture
{
    /// <summary>
    /// 运行入口：挂到任意空物体即可。左侧切换 12 个例子，一次只启用一个，避免 UI 重叠。
    /// 它只是示例导航，不是 NoArchitectureShopDemo 的控制层或业务依赖。
    /// </summary>
    public sealed class ArchitectureExamplesLauncher : MonoBehaviour
    {
        private readonly Type[] types =
        {
            typeof(ArchitectureExamples.NoArchitecture.NoArchitectureShopDemo),
            typeof(ArchitectureExamples.ClassicMvc.MvcShopDemo),
            typeof(ArchitectureExamples.PassiveMvp.PassiveMvpShopDemo),
            typeof(ArchitectureExamples.TaligentMvp.TaligentShopDemo),
            typeof(ArchitectureExamples.SupervisingMvp.SupervisingShopDemo),
            typeof(ArchitectureExamples.Presentation.PresentationModelShopDemo),
            typeof(ArchitectureExamples.ManualMvvm.ManualMvvmShopDemo),
            typeof(ArchitectureExamples.AutoMvvm.AutoMvvmShopDemo),
            typeof(ArchitectureExamples.DependencyGraph.DependencyGraphShopDemo),
            typeof(ArchitectureExamples.PushReactive.PushShopDemo),
            typeof(ArchitectureExamples.PullReactive.PullShopDemo),
            typeof(ArchitectureExamples.HybridReactive.HybridShopDemo),
        };
        private readonly string[] titles =
        {
            "00 | No architecture",
            "01 | Classic MVC",
            "02 | MVP: Passive View",
            "03 | MVP: Taligent concepts",
            "04 | MVP: Supervising Controller",
            "05 | Presentation Model",
            "06 | MVVM: manual binding",
            "07 | MVVM: UI Toolkit binding",
            "08 | Reactive: dependency graph",
            "09 | Reactive: immediate push",
            "10 | Reactive: lazy pull",
            "11 | Reactive: push + pull",
        };
        private MonoBehaviour[] demos;
        private GameObject host;
        private int selected;
        private Vector2 scroll;
        private void Awake()
        {
            host = new GameObject("Architecture examples (runtime)");
            host.transform.SetParent(transform, false);
            host.SetActive(false); // 添加组件时不触发各示例的 OnEnable。
            demos = new MonoBehaviour[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                demos[i] = (MonoBehaviour)host.AddComponent(types[i]);
                demos[i].enabled = false;
            }
            host.SetActive(true);
        }
        private void OnEnable() { if (demos != null) demos[selected].enabled = true; }
        private void OnDisable() { if (demos != null) demos[selected].enabled = false; }
        private void OnDestroy() { if (host != null) Destroy(host); }
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(12, 12, 260, Mathf.Max(150, Screen.height - 24)), GUI.skin.box);
            GUILayout.Label("Architecture comparison");
            GUILayout.Label("Same shop, different responsibilities");
            scroll = GUILayout.BeginScrollView(scroll);
            for (int i = 0; i < titles.Length; i++)
            {
                if (GUILayout.Button((i == selected ? "> " : "") + titles[i]))
                {
                    if (i == selected) continue;
                    demos[selected].enabled = false;
                    selected = i;
                    demos[selected].enabled = true;
                }
            }
            GUILayout.Label("Switching restarts that example.");
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }
}
