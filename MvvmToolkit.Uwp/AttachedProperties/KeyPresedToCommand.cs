using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MvvmToolkit.WinRT.AttachedProperties
{
    // TODO: add Key property (maybe)
    // TODO: add CommandParameter property
    public static class KeyPresedToCommand
    {
        public static ICommand GetCommand(Control control)
        {
            return (ICommand) control.GetValue(CommandProperty);
        }

        public static void SetCommand(Control control, ICommand value)
        {
            control.SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand),
            typeof(KeyPresedToCommand), new PropertyMetadata(null, CommandPropertyChanged));

        public static void CommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Control control)
            {
                control.KeyUp += OnKeyUp;
            }
        }

        private static void OnKeyUp(object s, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                ExecuteCommand(s);
            }
        }

        private static void ExecuteCommand(object element)
        {
            var command = GetCommand((Control) element);
            if (command != null && command.CanExecute(null))
            {
                command.Execute(null);
            }
        }
    }
}
