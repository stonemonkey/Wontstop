using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;

namespace Common.Uwp.StateTriggers
{
    public class CompositeStateTrigger : StateTriggerValueBase
    {
        public CompositeStateTrigger()
        {
            StateTriggers = new StateTriggerCollection();
        }

        private void EvaluateTriggers()
        {
            if (!StateTriggers.Any())
            {
                IsActive = false;
            }
            else if (Operator == LogicalOperator.Or)
            {
                bool result = GetValues().Any(t => t);
                IsActive = (result);
            }
            else if (Operator == LogicalOperator.And)
            {
                bool result = GetValues().All(t => t);
                IsActive = (result);
            }
            else if (Operator == LogicalOperator.OnlyOne)
            {
                bool result = GetValues().Count(t => t) == 1;
                IsActive = (result);
            }
        }

        private IEnumerable<bool> GetValues()
        {
            foreach (var trigger in StateTriggers)
            {
                var value = trigger as ITriggerValue;
                if (value != null)
                {
                    yield return value.IsActive;
                }
                else if (trigger is StateTrigger)
                {
                    yield return ((StateTrigger) trigger).IsActive;
                }
            }
        }

        private void CompositeTrigger_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnTriggerCollectionChanged(e.OldItems?.OfType<StateTriggerBase>(),
                e.OldItems == null ? null : e.NewItems.OfType<StateTriggerBase>());
            // TODO: handle reset
        }

        private void CompositeStateTrigger_VectorChanged(IObservableVector<DependencyObject> sender, IVectorChangedEventArgs e)
        {
            if (e.CollectionChange == CollectionChange.ItemInserted)
            {
                var item = sender[(int) e.Index] as StateTriggerBase;
                if (item != null)
                {
                    OnTriggerCollectionChanged(null, new[] { item });
                }
            }
            // TODO: else handle remove and reset
        }

        private void OnTriggerCollectionChanged(IEnumerable<StateTriggerBase> oldItems, IEnumerable<StateTriggerBase> newItems)
        {
            if (newItems != null)
            {
                foreach (var item in newItems)
                {
                    if (item is StateTrigger)
                    {
                        long id = item.RegisterPropertyChangedCallback(
                            StateTrigger.IsActiveProperty, TriggerIsActivePropertyChanged);
                        item.SetValue(RegistrationTokenProperty, id);
                    }
                    else if (item is ITriggerValue)
                    {
                        ((ITriggerValue) item).IsActiveChanged += CompositeTrigger_IsActiveChanged;
                    }
                    else
                    {
                        throw new NotSupportedException("Only StateTrigger or triggers implementing ITriggerValue are supported in a Composite trigger");
                    }
                }
            }
            if (oldItems != null)
            {
                foreach (var item in oldItems)
                {
                    if (item is StateTrigger)
                    {
                        var value = item.GetValue(RegistrationTokenProperty);
                        if ((value as long?) > 0)
                        {
                            item.ClearValue(RegistrationTokenProperty);
                            item.UnregisterPropertyChangedCallback(StateTrigger.IsActiveProperty, (long) value);
                        }
                    }
                    else if (item is ITriggerValue)
                    {
                        ((ITriggerValue) item).IsActiveChanged -= CompositeTrigger_IsActiveChanged;
                    }
                }
            }
            EvaluateTriggers();
        }

        private void CompositeTrigger_IsActiveChanged(object sender, EventArgs e)
        {
            EvaluateTriggers();
        }

        private void TriggerIsActivePropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            EvaluateTriggers();
        }

        public StateTriggerCollection StateTriggers
        {
            get { return (StateTriggerCollection) GetValue(StateTriggersProperty); }
            set { SetValue(StateTriggersProperty, value); }
        }

        private static readonly DependencyProperty StateTriggersProperty =
            DependencyProperty.Register("StateTriggers", typeof(StateTriggerCollection), typeof(CompositeStateTrigger), new PropertyMetadata(null, OnStateTriggersPropertyChanged));

        private static void OnStateTriggersPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trigger = (CompositeStateTrigger)d;
            var value = e.OldValue as INotifyCollectionChanged;
            if (value != null)
            {
                value.CollectionChanged -= trigger.CompositeTrigger_CollectionChanged;
            }
            else if (e.OldValue is IObservableVector<DependencyObject>)
            {
                ((IObservableVector<DependencyObject>) e.OldValue).VectorChanged += trigger.CompositeStateTrigger_VectorChanged;
                trigger.OnTriggerCollectionChanged((e.NewValue as IObservableVector<DependencyObject>).OfType<StateTriggerBase>(), null);
            }

            if (e.NewValue is INotifyCollectionChanged)
            {
                //TODO: Should be weak reference just in case
                (e.NewValue as INotifyCollectionChanged).CollectionChanged += trigger.CompositeTrigger_CollectionChanged;
            }
            else if (e.NewValue is IObservableVector<DependencyObject>)
            {
                ((IObservableVector<DependencyObject>) e.NewValue).VectorChanged += trigger.CompositeStateTrigger_VectorChanged;
                trigger.OnTriggerCollectionChanged(null, ((IObservableVector<DependencyObject>) e.NewValue).OfType<StateTriggerBase>());
            }
            if (e.NewValue is IEnumerable<StateTriggerBase>)
            {
                foreach (var item in e.NewValue as IEnumerable<StateTriggerBase>)
                {
                    if (!(item is StateTrigger || !(item is ITriggerValue)))
                    {
                        try
                        {
                            throw new NotSupportedException("Only StateTrigger or triggers implementing ITriggerValue are supported in a Composite trigger");
                        }
                        finally
                        {
                            trigger.SetValue(StateTriggersProperty, e.OldValue);
                        }
                    }
                }
                trigger.CompositeTrigger_CollectionChanged(e.NewValue,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (e.NewValue as IEnumerable<StateTriggerBase>).ToList()));
            }
            trigger.EvaluateTriggers();
        }

        private static readonly DependencyProperty RegistrationTokenProperty =
            DependencyProperty.RegisterAttached("RegistrationToken", typeof(long), typeof(CompositeStateTrigger), new PropertyMetadata(0));

        public LogicalOperator Operator
        {
            get { return (LogicalOperator)GetValue(OperatorProperty); }
            set { SetValue(OperatorProperty, value); }
        }

        public static readonly DependencyProperty OperatorProperty =
            DependencyProperty.Register("Operator", typeof(LogicalOperator), typeof(CompositeStateTrigger), new PropertyMetadata(LogicalOperator.And, OnEvaluatePropertyChanged));

        private static void OnEvaluatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trigger = (CompositeStateTrigger) d;
            trigger.EvaluateTriggers();
        }
    }

    public enum LogicalOperator
    {
        OnlyOne,
        And,
        Or,
    }

    public sealed class StateTriggerCollection : DependencyObjectCollection { }
}

