using System;

namespace Common.Uwp.StateTriggers
{
    public interface ITriggerValue
    {
        bool IsActive { get; }

        event EventHandler IsActiveChanged;
    }
}