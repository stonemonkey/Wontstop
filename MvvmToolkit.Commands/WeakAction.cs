using System;
using System.Reflection;

namespace MvvmToolkit.Commands
{
    public class WeakAction
    {
        private Action _staticAction;

        protected MethodInfo Method { get; set; }

        public virtual string MethodName
        {
            get
            {
                if (_staticAction != null)
                {
                    return _staticAction.GetMethodInfo().Name;
                }

                return Method.Name;
            }
        }

        protected WeakReference ActionReference { get; set; }

        protected WeakReference Reference { get; set; }

        public bool IsStatic => _staticAction != null;

        protected WeakAction()
        {
        }

        public WeakAction(Action action)
            : this(action?.Target, action)
        {
        }

        public WeakAction(object target, Action action)
        {
            if (action.GetMethodInfo().IsStatic)
            {
                _staticAction = action;

                if (target != null)
                {
                    Reference = new WeakReference(target);
                }

                return;
            }

            Method = action.GetMethodInfo();

            ActionReference = new WeakReference(action.Target);

            Reference = new WeakReference(target);
        }

        public virtual bool IsAlive
        {
            get
            {
                if ((_staticAction == null) && (Reference == null))
                {
                    return false;
                }

                if (_staticAction != null)
                {
                    if (Reference != null)
                    {
                        return Reference.IsAlive;
                    }

                    return true;
                }

                return Reference.IsAlive;
            }
        }

        public object Target => Reference?.Target;

        protected object ActionTarget => ActionReference?.Target;

        public void Execute()
        {
            if (_staticAction != null)
            {
                _staticAction();
                return;
            }

            var actionTarget = ActionTarget;
            if (IsAlive && (Method != null) && (ActionReference != null) && (actionTarget != null))
            {
                Method.Invoke(actionTarget, null);
            }
        }

        public void MarkForDeletion()
        {
            Reference = null;
            ActionReference = null;
            Method = null;
            _staticAction = null;
        }
    }

    public class WeakAction<T> : WeakAction, IExecuteWithObject
    {
        private Action<T> _staticAction;

        public override string MethodName
        {
            get
            {
                if (_staticAction != null)
                {
                    return _staticAction.GetMethodInfo().Name;
                }

                return Method.Name;
            }
        }

        public override bool IsAlive
        {
            get
            {
                if ((_staticAction == null) && (Reference == null))
                {
                    return false;
                }

                if (_staticAction != null)
                {
                    if (Reference != null)
                    {
                        return Reference.IsAlive;
                    }

                    return true;
                }

                return Reference.IsAlive;
            }
        }

        public WeakAction(Action<T> action)
            : this(action?.Target, action)
        {
        }

        public WeakAction(object target, Action<T> action)
        {
            if (action.GetMethodInfo().IsStatic)
            {
                _staticAction = action;

                if (target != null)
                {
                    Reference = new WeakReference(target);
                }

                return;
            }

            Method = action.GetMethodInfo();

            ActionReference = new WeakReference(action.Target);

            Reference = new WeakReference(target);
        }

        public new void Execute()
        {
            Execute(default(T));
        }

        public void Execute(T parameter)
        {
            if (_staticAction != null)
            {
                _staticAction(parameter);
                return;
            }

            var actionTarget = ActionTarget;

            if (IsAlive && (Method != null) && (ActionReference != null) && (actionTarget != null))
            {
                Method.Invoke(actionTarget, new object[] { parameter });
            }
        }

        public void ExecuteWithObject(object parameter)
        {
            var parameterCasted = (T) parameter;
            Execute(parameterCasted);
        }

        public new void MarkForDeletion()
        {
            _staticAction = null;
            base.MarkForDeletion();
        }
    }
}
