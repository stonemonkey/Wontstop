using System;
using System.Reflection;
using System.Windows.Input;

namespace MvvmToolkit.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly WeakAction _execute;

        private readonly WeakFunc<bool> _canExecute;

        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            _execute = new WeakAction(execute);

            if (canExecute != null)
            {
                _canExecute = new WeakFunc<bool>(canExecute);
            }
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return (_canExecute == null) || (_canExecute.IsStatic || _canExecute.IsAlive) && _canExecute.Execute();
        }

        public virtual void Execute(object parameter)
        {
            if (CanExecute(parameter) && (_execute != null) && (_execute.IsStatic || _execute.IsAlive))
            {
                _execute.Execute();
            }
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly WeakAction<T> _execute;

        private readonly WeakFunc<T, bool> _canExecute;

        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            _execute = new WeakAction<T>(execute);

            if (canExecute != null)
            {
                _canExecute = new WeakFunc<T, bool>(canExecute);
            }
        }

        public event EventHandler CanExecuteChanged;
        
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            if (_canExecute.IsStatic || _canExecute.IsAlive)
            {
                if (parameter == null && typeof(T).GetTypeInfo().IsValueType)
                {
                    return _canExecute.Execute(default(T));
                }

                return _canExecute.Execute((T) parameter);
            }

            return false;
        }

        public virtual void Execute(object parameter)
        {
            if (CanExecute(parameter) && _execute != null && (_execute.IsStatic || _execute.IsAlive))
            {
                if (parameter == null)
                {
                    if (typeof(T).GetTypeInfo().IsValueType)
                    {
                        _execute.Execute(default(T));
                    }
                    else
                    {
                        _execute.Execute((T) parameter);
                    }
                }
                else
                {
                    _execute.Execute((T) parameter);
                }
            }
        }
    }
}