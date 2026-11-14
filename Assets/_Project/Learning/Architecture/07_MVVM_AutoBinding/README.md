# MVVM / 自动绑定

对应参考图：05_MVVM_Binding_Variants.png 左半。

实际使用 Unity 6 UI Toolkit DataBinding；引擎完成属性到控件的同步。

## 阅读顺序

ShopViewModel.cs -> AutoMvvmShopDemo.cs

## 数据流

Command -> Model；VM.propertyChanged -> DataBinding -> 控件属性


## 统一业务

商品单价初始 100，数量初始 2，余额初始 500，已拥有数量初始 0。数量限制在 1..5，单价可以切换为 100/150。总价为单价乘数量；余额足够才允许购买，购买时扣除总价并增加已拥有数量。Reset 恢复初始状态。

所有真正的购买入口都会校验余额，不能只靠禁用按钮保护规则。
“Change together”在同一个点击处理函数内把单价改为 150、数量改为 3，用来观察多源更新。它不是网络请求，也不模拟异步加载。

## 运行与完整对照

见 [总说明与运行步骤](../00_NoArchitecture/README.md)。

实际采用 UI Toolkit DataBinding + INotifyBindablePropertyChanged + CreateProperty。Resources 下的主题用于运行时控件样式；PurchaseButton.CanPress 将绑定值适配到 SetEnabled。输入命令显式连接，属性输出由引擎自动绑定；此例不是双向绑定。
