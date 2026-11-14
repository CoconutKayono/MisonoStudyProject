using System;

namespace ArchitectureExamples.TaligentMvp
{
    public enum ShopGesture { Less, More, TogglePrice, Buy, ChangeOffer, Reset }

    /// <summary>
    /// Interactor 解释手势；示例把 Unity 按钮点击视为手势，不重新实现鼠标命中检测。
    /// 它产生 Command；Presenter 负责提供 Selection 并协调执行。
    /// </summary>
    public sealed class ShopInteractor
    {
        public event Action<ShopCommand> CommandRequested;
        public void Interpret(ShopGesture gesture)
        {
            // 名称相同但角色不同：Gesture 是输入含义，Operation 是要执行的操作。
            var operation = (ShopOperation)Enum.Parse(typeof(ShopOperation), gesture.ToString());
            CommandRequested?.Invoke(new ShopCommand(operation));
        }
    }
}
