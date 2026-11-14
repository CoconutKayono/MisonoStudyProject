# 响应式 / Push-Pull

对应参考图：08_Reactive_Push_Pull.png 右列。

推送失效、批量去重副作用、读取时计算；同一次操作只消费最终值。

## 阅读顺序

ReactiveShopModel.cs -> HybridShopDemo.cs；机制代码在 08

## 数据流

Batch 写两个源 -> Dirty + 去重 -> Flush -> 一次计算 450 -> UI


## 统一业务

商品单价初始 100，数量初始 2，余额初始 500，已拥有数量初始 0。数量限制在 1..5，单价可以切换为 100/150。总价为单价乘数量；余额足够才允许购买，购买时扣除总价并增加已拥有数量。Reset 恢复初始状态。

所有真正的购买入口都会校验余额，不能只靠禁用按钮保护规则。
“Change together”在同一个点击处理函数内把单价改为 150、数量改为 3，用来观察多源更新。它不是网络请求，也不模拟异步加载。

## 运行与完整对照

见 [总说明与运行步骤](../00_NoArchitecture/README.md)。

本例复用 08_Reactive_DependencyGraph 中的 ReactiveRuntime、ReactiveProperty、Computed、ReactiveEffect。08 演示多 Effect 依赖图；本例集中展示一次批处理只执行一次 UI Effect。
