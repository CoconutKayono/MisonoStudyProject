# MVP / Taligent 六角色

对应参考图：04_MVP_Taligent_Supervising.png 左半。

把交互解释、命令、选择目标拆开；保留 Model/View/Presenter。

## 阅读顺序

ShopSelection.cs -> ShopInteractor.cs -> ShopCommand.cs -> ShopPresenter.cs

## 数据流

Gesture -> Interactor -> CommandRequested -> Presenter -> Command.Execute(Selection) -> Model


## 统一业务

商品单价初始 100，数量初始 2，余额初始 500，已拥有数量初始 0。数量限制在 1..5，单价可以切换为 100/150。总价为单价乘数量；余额足够才允许购买，购买时扣除总价并增加已拥有数量。Reset 恢复初始状态。

所有真正的购买入口都会校验余额，不能只靠禁用按钮保护规则。
“Change together”在同一个点击处理函数内把单价改为 150、数量改为 3，用来观察多源更新。它不是网络请求，也不模拟异步加载。

## 运行与完整对照

见 [总说明与运行步骤](../00_NoArchitecture/README.md)。

Selection 显式保存当前商品目标，Interactor 将按钮手势解释为 Command，Presenter 协调 Command 在 Selection 上执行。额外的 Select / clear product 按钮用于观察“没有选择目标”的情况。此例只映射六个角色，不完整复刻历史框架。
