using System;

namespace ArchitectureExamples.AutoMvvm
{
    /// <summary>最小 UI 命令对象：执行前再次检查 CanExecute。它不是 Taligent 的历史 Command 实现。</summary>
    public sealed class ShopCommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;
        public ShopCommand(Action execute, Func<bool> canExecute = null)
        { this.execute = execute; this.canExecute = canExecute ?? (() => true); }
        public bool CanExecute => canExecute();
        public void Execute() { if (CanExecute) execute(); }
    }
}
