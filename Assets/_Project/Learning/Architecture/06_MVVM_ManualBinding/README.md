# MVVM / 手动绑定

对应参考图：05_MVVM_Binding_Variants.png 右半。

ViewModel 暴露属性和命令；Binding 手动监听 INotifyPropertyChanged 并刷新。

## 阅读顺序

ShopViewModel.cs -> ShopBinding.cs -> ManualMvvmShopDemo.cs

## 数据流

Command -> Model；Changed -> VM.PropertyChanged -> Binding 读取属性 -> View.Render


## 统一业务

商品单价初始 100，数量初始 2，余额初始 500，已拥有数量初始 0。数量限制在 1..5，单价可以切换为 100/150。总价为单价乘数量；余额足够才允许购买，购买时扣除总价并增加已拥有数量。Reset 恢复初始状态。

所有真正的购买入口都会校验余额，不能只靠禁用按钮保护规则。
“Change together”在同一个点击处理函数内把单价改为 150、数量改为 3，用来观察多源更新。它不是网络请求，也不模拟异步加载。

## 运行与完整对照

见 [总说明与运行步骤](../00_NoArchitecture/README.md)。

ShopBinding 手动接收标准 INotifyPropertyChanged 通知并同步 View，IMGUI 替代 UGUI 减少场景配置；不包含自动依赖收集，也没有使用 UI Toolkit 绑定引擎。
