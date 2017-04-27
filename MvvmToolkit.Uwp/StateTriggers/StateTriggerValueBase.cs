using System;
using Windows.UI.Xaml;

namespace MvvmToolkit.Uwp.StateTriggers
{
    public abstract class StateTriggerValueBase : StateTriggerBase, ITriggerValue
    {
        private bool _isActive;

        public bool IsActive
        {
            get { return _isActive; }
            protected set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    SetActive(value);
                    IsActiveChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler IsActiveChanged;
    }
}