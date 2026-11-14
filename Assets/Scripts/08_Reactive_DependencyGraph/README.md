# 响应式 / 自动依赖图

对应参考图：07_Reactive_Dependency_Graph.png。

读取自动 Track；写入传播 Dirty；Batch 后 Effect 去重执行，读取时重算。

## 阅读顺序

ReactiveProperty.cs -> Computed.cs -> ReactiveEffect.cs -> ReactiveShopModel.cs

## 数据流

源 -> Dirty 传播 -> Effect 队列 -> Batch 结束 -> Effect 读取 Computed -> UI


## 统一业务

商品单价初始 100，数量初始 2，余额初始 500，已拥有数量初始 0。数量限制在 1..5，单价可以切换为 100/150。总价为单价乘数量；余额足够才允许购买，购买时扣除总价并增加已拥有数量。Reset 恢复初始状态。

所有真正的购买入口都会校验余额，不能只靠禁用按钮保护规则。
“Change together”在同一个点击处理函数内把单价改为 150、数量改为 3，用来观察多源更新。它不是网络请求，也不模拟异步加载。

## 运行与完整对照

见 [总说明与运行步骤](../00_NoArchitecture/README.md)。

Computed.EvaluationCount 和 ReactiveEffect.RunCount 可观察缓存和调度次数。getter Track 自动收集依赖，分支切换会解除旧依赖；Dispose 释放订阅。作用域为单个运行时实例，限 Unity 主线程。
