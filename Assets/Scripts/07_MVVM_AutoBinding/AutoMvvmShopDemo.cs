using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace ArchitectureExamples.AutoMvvm
{
    /// <summary>
    /// 对应图 05 左：真正使用 Unity 6 的 DataBinding。属性 -> 控件由引擎同步。
    /// UI Toolkit 不自动把普通按钮事件变成命令，因此点击到 Command 的映射仍显式注册一次。
    /// </summary>
    public sealed class AutoMvvmShopDemo : MonoBehaviour
    {
        private ShopViewModel viewModel;
        private GameObject documentObject;
        private PanelSettings settings;

        private void OnEnable()
        {
            viewModel = new ShopViewModel(new ShopModel());
            settings = ScriptableObject.CreateInstance<PanelSettings>();
            settings.themeStyleSheet = Resources.Load<ThemeStyleSheet>("ArchitectureRuntimeTheme");
            documentObject = new GameObject("Automatic binding UI");
            documentObject.transform.SetParent(transform, false);
            var document = documentObject.AddComponent<UIDocument>();
            document.panelSettings = settings;
            var root = document.rootVisualElement;
            root.pickingMode = PickingMode.Ignore; // 透明区域不遮挡左侧 IMGUI 导航。
            var card = new VisualElement();
            card.style.position = Position.Absolute;
            card.style.left = 290; card.style.top = 12;
            card.style.width = 600; card.style.paddingLeft = 16; card.style.paddingRight = 16;
            card.style.paddingTop = 16; card.style.paddingBottom = 16;
            card.style.backgroundColor = new Color(0.16f, 0.18f, 0.22f);
            card.style.color = Color.white;
            card.dataSource = viewModel; // 子控件继承数据源。
            root.Add(card);
            card.Add(new Label("07 | MVVM: Unity UI Toolkit automatic binding"));
            var summary = new Label();
            summary.style.whiteSpace = WhiteSpace.Normal;
            Bind(summary, "text", nameof(ShopViewModel.Summary));
            card.Add(summary);
            AddButton(card, "Count -", viewModel.LessCommand);
            AddButton(card, "Count +", viewModel.MoreCommand);
            AddButton(card, "Price 100 / 150", viewModel.TogglePriceCommand);
            var buy = new PurchaseButton();
            buy.clicked += viewModel.BuyCommand.Execute;
            Bind(buy, "text", nameof(ShopViewModel.BuyText));
            Bind(buy, nameof(PurchaseButton.CanPress), nameof(ShopViewModel.CanBuy));
            card.Add(buy);
            AddButton(card, "Change together: Price=150, Count=3", viewModel.ChangeOfferCommand);
            AddButton(card, "Reset", viewModel.ResetCommand);
            card.Add(new Label("Property updates are applied by DataBinding, without a manual Render call."));
        }
        private static void Bind(VisualElement target, string targetProperty, string sourceProperty)
        {
            target.SetBinding(targetProperty, new DataBinding
            {
                dataSourcePath = new PropertyPath(sourceProperty),
                bindingMode = BindingMode.ToTarget,
                updateTrigger = BindingUpdateTrigger.OnSourceChanged
            });
        }
        private static void AddButton(VisualElement card, string text, ShopCommand command)
        { card.Add(new Button(command.Execute) { text = text }); }
        private void OnDisable()
        {
            // 先使 UI 脱离面板，再销毁临时对象；避免切换示例时残留绑定和按钮回调。
            if (documentObject != null) { documentObject.SetActive(false); Destroy(documentObject); }
            if (settings != null) Destroy(settings);
            viewModel?.Dispose();
        }
    }

    /// <summary>enabledSelf 是只读属性；这个可写适配属性让绑定引擎可以调用 SetEnabled。</summary>
    public sealed class PurchaseButton : Button
    {
        [CreateProperty]
        public bool CanPress { get => enabledSelf; set => SetEnabled(value); }
    }
}
