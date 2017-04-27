using System;

namespace MvvmToolkit.Uwp.StateTriggers
{
    public interface ITriggerValue
    {
        bool IsActive { get; }

        event EventHandler IsActiveChanged;
    }
}