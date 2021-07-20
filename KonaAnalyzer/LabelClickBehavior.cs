using System;
using System.Windows.Input;
using ReactiveUI;
using Xamarin.Forms;

namespace KonaAnalyzer
{
    public static class ReactiveExtensions
    {
        public static ReactiveCommand<T, T2> OnExecuting<T, T2>(this ReactiveCommand<T, T2> command, Action<bool> act)
        {
            command.IsExecuting.Subscribe(act);
            return command;
        }

        public static ReactiveCommand<T, T2> OnCompletion<T, T2>(this ReactiveCommand<T, T2> command, Action<T2> act)
        {
            command.Subscribe(act);
            return command;
        }
        public static ReactiveCommand<T, T2> OnCompletion<T, T2>(this ReactiveCommand<T, T2> command, ICommand executeCommand)
        {
            command.InvokeCommand(executeCommand);
            return command;
        }

        public static ReactiveCommand<T, T2> OnException<T, T2>(this ReactiveCommand<T, T2> command, Action<Exception> exception)
        {
            command.ThrownExceptions.Subscribe(exception);
            return command;
        }
    }
    public class LabelClickBehavior : Behavior<Label>
    {
        protected override void OnAttachedTo(BindableObject bindable)
        {
            base.OnAttachedTo(bindable);

        }
    }
}