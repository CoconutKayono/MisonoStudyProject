# Unity 架构与响应式编程：同一个商店，12 种对照

## 如何运行

1. 等 Unity 编译结束，点击菜单 **Tools > Architecture Examples > Create Launcher**。
2. 点击 **Play**，在 Game 窗口左侧选择示例。推荐窗口至少 960 × 650。
3. 每次切换会停用旧示例、解除订阅，并把新示例恢复为初始状态。每份示例各有独立数据。

也可以将本文件夹的 **ArchitectureExamplesLauncher.cs** 拖到任意空 GameObject 上，再 Play。
若只研究一个例子，可单独挂该文件夹中的 **...ShopDemo.cs**；面板显示在右侧预留区域。
导航脚本只是运行入口，不参与任何示例的业务数据流；**NoArchitectureShopDemo.cs** 自身仍是一个不分层的完整实现。

无需手工创建 Canvas、Button 或 TMP。除自动绑定版使用 UI Toolkit 外，其余使用 IMGUI，目的是减少场景与控件配置对架构阅读的干扰。UI 文案使用英文以避免默认字体缺少中文字形；代码注释和学习说明为中文。

## 统一业务

商品单价初始 100，数量初始 2，余额初始 500，已拥有数量初始 0。数量限制在 1..5，单价可以切换为 100/150。总价为单价乘数量；余额足够才允许购买，购买时扣除总价并增加已拥有数量。Reset 恢复初始状态。

所有真正的购买入口都会校验余额，不能只靠禁用按钮保护规则。
“Change together”在同一个点击处理函数内把单价改为 150、数量改为 3，用来观察多源更新。它不是网络请求，也不模拟异步加载。

## 文件夹与阅读顺序

| 文件夹 | 对照点 |
|---|---|
| 00_NoArchitecture | 无架构：数据、业务校验、文案、按钮状态、输入处理都在一个脚本中。 |
| 01_MVC_Smalltalk | 经典 MVC：View 自己观察 Model；Controller 解释输入，不主动 Render View。 |
| 02_MVP_PassiveView | MVP / Passive View：View 不读取 Model；Presenter 推导并推送全部 ShopViewState。 |
| 03_MVP_Taligent | MVP / Taligent 六角色：把交互解释、命令、选择目标拆开；保留 Model/View/Presenter。 |
| 04_MVP_Supervising | MVP / Supervising Controller：View 直接绑定简单字段；Presenter 协调组合文案和购买状态。 |
| 05_PresentationModel | Presentation Model：把文案、可点击状态从 View 抽到无控件引用的展示模型中；本例轮询同步。 |
| 06_MVVM_ManualBinding | MVVM / 手动绑定：ViewModel 暴露属性和命令；Binding 手动监听 INotifyPropertyChanged 并刷新。 |
| 07_MVVM_AutoBinding | MVVM / 自动绑定：实际使用 Unity 6 UI Toolkit DataBinding；引擎完成属性到控件的同步。 |
| 08_Reactive_DependencyGraph | 响应式 / 自动依赖图：读取自动 Track；写入传播 Dirty；Batch 后 Effect 去重执行，读取时重算。 |
| 09_Reactive_Push | 响应式 / Push：显式依赖，写入后立即计算、立即通知 UI。 |
| 10_Reactive_Pull | 响应式 / Pull：写入只置 Dirty；消费者点击 Read 才重算并缓存。 |
| 11_Reactive_PushPull | 响应式 / Push-Pull：推送失效、批量去重副作用、读取时计算；同一次操作只消费最终值。 |

先读 00，再对照 01/02/04 的“谁读取 Model、谁计算展示状态、谁通知谁”。03 单独展示 Taligent 六角色。05/06/07 对比展示模型与同步机制。最后读 08 的机制，再对照 09/10/11 的调度策略。

## 与 8 张参考图的对应

| 参考图文件名 | 对应示例 |
|---|---|
| 01_MVC_Smalltalk80.png | 01_MVC_Smalltalk |
| 02_MVC_Web.png | 按要求省略 |
| 03_MVP_PassiveView.png | 02_MVP_PassiveView |
| 04_MVP_Taligent_Supervising.png | 03_MVP_Taligent、04_MVP_Supervising |
| 05_MVVM_Binding_Variants.png | 06_MVVM_ManualBinding、07_MVVM_AutoBinding |
| 06_PresentationModel_Evolution.png | 01、05、06、07 |
| 07_Reactive_Dependency_Graph.png | 08_Reactive_DependencyGraph |
| 08_Reactive_Push_Pull.png | 09、10、11 |

图里的应用服务层在本例中合并为模型方法：业务只有一次同步购买，额外包装服务会增加阅读成本。若增加支付、库存和网络调用，可再引入用例服务协调它们。

## 最值得实际点击的对比

- **01 / 02**：MVC 的 View 订阅 Model；Passive MVP 的 Presenter 订阅 Model，View 仅接收 Render。
- **02 / 04**：Passive View 的展示全部由 Presenter 提供；Supervising 的简单数据显示由 View 直接处理。
- **03**：点击 Select / clear product。取消选择后各 Command 不修改数据；重新选择后恢复。Selection 不是商品数据本身。
- **06 / 07**：手动版的 ShopBinding 中有 Render；自动版通过 SetBinding 注册绑定，不手工订阅 VM 通知后刷新控件。按钮点击到 Command 的映射仍显式注册。
- **09**：Reset 后点击 Change together，计算/消费轨迹为 **200 -> 300 -> 450**。这是同一个点击处理函数内的两次通知；屏幕未必真的显示一个中间帧，但订阅者确实消费过 300。
- **10**：Reset 后点击 Change together，源变为 150/3，但派生计算次数不增加，Dirty=true；点击 **Read derived values now (Pull)** 后才算出 450，轨迹为 **200 -> 450**。顶部与按钮保留“上次读取的快照”；购买入口会先检查最新数据。
- **11**：Reset 后点击 Change together，无需手动 Read，界面直接消费 450，轨迹为 **200 -> 450**；Total 只新增一次计算，UI Effect 只新增一次执行。

08/11 的累计计算次数不因 Reset 归零；Reset 只清空数值轨迹，观察操作前后的次数差即可。

## 机制边界

- MVC、MVP、Presentation Model、MVVM 主要讨论职责分配；响应式讨论变化如何传播、何时计算。它们可以组合。
- Changed / PropertyChanged 是通知，读取属性才是取数。不要把通知方向与数据读取方向混为一谈。
- 自动依赖追踪、MVVM 自动绑定、双向绑定是不同概念。07 使用单向 ToTarget 展示绑定，输入用 Command；不需要双向绑定也可以是 MVVM。
- 06 为了零场景配置使用 IMGUI 模拟手动控件同步，而非声称已经搭建 UGUI。07 是实际的 Unity 6 UI Toolkit 运行时绑定。
- 03 是 Taligent 六概念的最小教学映射，不包含原框架的全部交互器、命令历史、撤销或范围选择。
- 08 是教学用同步信号/依赖图，不是 UniRx/R3 实现，也不是完整 IObservable 流库。Effect 应只消费数据并更新展示，不应写入自己读取的源。
- 08/11 的源修改同步完成完整 Dirty 传播，再 Flush Effect，以避免菱形依赖读到半新半旧缓存；显式 Batch 合并多源写入。纯 Pull 的按需计算不等于每帧轮询。
- 各示例刻意保留少量相同 UI/领域代码，方便独立对比。11 明确复用 08 的响应式运行时；删除 08 会影响 11。
- 切换和禁用会释放事件订阅、Effect 与临时 UIDocument。重复运行不依赖静态单例保存数据。

## 资料

用户提供的参考图用于确定示例覆盖范围。术语与实现还参考以下原始资料：

- [Martin Fowler：GUI Architectures](https://martinfowler.com/eaaDev/uiArchs.html)
- [Martin Fowler：Passive View](https://martinfowler.com/eaaDev/PassiveScreen.html)
- [Martin Fowler：Supervising Controller](https://martinfowler.com/eaaDev/SupervisingPresenter.html)
- [Mike Potel：Taligent MVP 原论文](https://www.wildcrest.com/Potel/Portfolio/mvp.pdf)
- [Microsoft：MVVM](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm)
- [Unity：运行时数据绑定](https://docs.unity3d.com/6000.0/Documentation/Manual/UIE-runtime-binding-types.html)
- [Vue：Reactivity in Depth](https://vuejs.org/guide/extras/reactivity-in-depth.html)
