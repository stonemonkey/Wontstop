using System;
using System.Reflection;

namespace MvvmToolkit.Commands
{
    public class WeakFunc<TResult>
    {
        private Func<TResult> _staticFunc;

        protected MethodInfo Method { get; set; }

        public bool IsStatic => _staticFunc != null;

        public virtual string MethodName
        {
            get
            {
                if (_staticFunc != null)
                {
                    return _staticFunc.GetMethodInfo().Name;
                }

                return Method.Name;
            }
        }

        protected WeakReference FuncReference { get; set; }

        protected WeakReference Reference { get; set; }

        protected WeakFunc()
        {
        }

        public WeakFunc(Func<TResult> func)
            : this(func?.Target, func)
        {
        }

        public WeakFunc(object target, Func<TResult> func)
        {
            if (func.GetMethodInfo().IsStatic)
            {
                _staticFunc = func;

                if (target != null)
                {
                    // keep a reference to the target to control the WeakAction's lifetime
                    Reference = new WeakReference(target);
                }

                return;
            }

            Method = func.GetMethodInfo();

            FuncReference = new WeakReference(func.Target);

            Reference = new WeakReference(target);
        }

        public virtual bool IsAlive
        {
            get
            {
                if ((_staticFunc == null) && (Reference == null))
                {
                    return false;
                }

                if (_staticFunc != null)
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

        protected object FuncTarget => FuncReference?.Target;

        public TResult Execute()
        {
            if (_staticFunc != null)
            {
                return _staticFunc();
            }

            var funcTarget = FuncTarget;
            if (IsAlive && (Method != null) && (FuncReference != null) && (funcTarget != null))
            {
                return (TResult) Method.Invoke(funcTarget, null);
            }

            return default(TResult);
        }

        public void MarkForDeletion()
        {
            Reference = null;
            FuncReference = null;
            Method = null;
            _staticFunc = null;
        }
    }

    public class WeakFunc<T, TResult> : WeakFunc<TResult>, IExecuteWithObjectAndResult
    {
        private Func<T, TResult> _staticFunc;

        public override string MethodName
        {
            get
            {
                if (_staticFunc != null)
                {
                    return _staticFunc.GetMethodInfo().Name;
                }

                return Method.Name;
            }
        }

        public override bool IsAlive
        {
            get
            {
                if ((_staticFunc == null) && (Reference == null))
                {
                    return false;
                }

                if (_staticFunc != null)
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

        public WeakFunc(Func<T, TResult> func)
            : this(func?.Target, func)
        {
        }

        public WeakFunc(object target, Func<T, TResult> func)
        {
            if (func.GetMethodInfo().IsStatic)
            {
                _staticFunc = func;

                if (target != null)
                {
                    // keep a reference to the target to control the WeakAction's lifetime
                    Reference = new WeakReference(target);
                }

                return;
            }

            Method = func.GetMethodInfo();

            FuncReference = new WeakReference(func.Target);

            Reference = new WeakReference(target);
        }

        public new TResult Execute()
        {
            return Execute(default(T));
        }

        public TResult Execute(T parameter)
        {
            if (_staticFunc != null)
            {
                return _staticFunc(parameter);
            }

            var funcTarget = FuncTarget;

            if (IsAlive && (Method != null) && (FuncReference != null) && (funcTarget != null))
            {
                return (TResult) Method.Invoke(funcTarget, new object[] { parameter });
            }

            return default(TResult);
        }

        public object ExecuteWithObject(object parameter)
        {
            var parameterCasted = (T) parameter;
            return Execute(parameterCasted);
        }

        public new void MarkForDeletion()
        {
            _staticFunc = null;
            base.MarkForDeletion();
        }
    }
}