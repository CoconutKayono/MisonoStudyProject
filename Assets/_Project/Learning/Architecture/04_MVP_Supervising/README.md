# MVP / Supervising Controller

对应参考图：04_MVP_Taligent_Supervising.png 右半。

View 直接绑定简单字段；Presenter 协调组合文案和购买状态。

## 阅读顺序

SupervisingShopDemo.cs -> ShopPresenter.cs

## 数据流

简单值：Model -> View；组合展示：Model -> Presenter -> View


## 统一业务

商品单价初始 100，数量初始 2，余额初始 500，已拥有数量初始 0。数量限制在 1..5，单价可以切换为 100/150。总价为单价乘数量；余额足够才允许购买，购买时扣除总价并增加已拥有数量。Reset 恢复初始状态。

所有真正的购买入口都会校验余额，不能只靠禁用按钮保护规则。
“Change together”在同一个点击处理函数内把单价改为 150、数量改为 3，用来观察多源更新。它不是网络请求，也不模拟异步加载。

## 运行与完整对照

见 [总说明与运行步骤](../00_NoArchitecture/README.md)。

