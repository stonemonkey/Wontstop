using System;

namespace MvvmToolkit.Uwp.StateTriggers
{
    public class WeakEventListener<TInstance, TSource, TEventArgs> where TInstance : class
    {
        private readonly WeakReference _weakInstance;

        public Action<TInstance, TSource, TEventArgs> OnEventAction { get; set; }

        public Action<TInstance, WeakEventListener<TInstance, TSource, TEventArgs>> OnDetachAction { get; set; }

        public WeakEventListener(TInstance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            _weakInstance = new WeakReference(instance);
        }

        public void OnEvent(TSource source, TEventArgs eventArgs)
        {
            var target = (TInstance) _weakInstance.Target;
            if (target != null)
            {
                OnEventAction?.Invoke(target, source, eventArgs);
            }
            else
            {
                Detach();
            }
        }

        public void Detach()
        {
            var target = (TInstance) _weakInstance.Target;
            if (OnDetachAction != null)
            {
                OnDetachAction(target, this);
                OnDetachAction = null;
            }
        }
    }

    internal class WeakEventListener<TInstance, TSource> where TInstance : class
    {
        private readonly WeakReference _weakInstance;

        public Action<TInstance, TSource> OnEventAction { get; set; }

        public Action<TInstance, WeakEventListener<TInstance, TSource>> OnDetachAction { get; set; }

        public WeakEventListener(TInstance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            _weakInstance = new WeakReference(instance);
        }

        public void OnEvent(TSource source)
        {
            var target = (TInstance) _weakInstance.Target;
            if (target != null)
            {
                OnEventAction?.Invoke(target, source);
            }
            else
            {
                Detach();
            }
        }

        public void Detach()
        {
            var target = (TInstance) _weakInstance.Target;
            if (OnDetachAction != null)
            {
                OnDetachAction(target, this);
                OnDetachAction = null;
            }
        }
    }
}
