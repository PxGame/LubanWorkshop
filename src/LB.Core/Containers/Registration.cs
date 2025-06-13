using Microsoft.Win32;
using System;
using System.Reflection;
using static LB.Core.Containers.IRegistration;

namespace LB.Core.Containers
{
    internal class Registration : IRegistration
    {
        public class ResolvingScope : IDisposable
        {
            private Registration _regist;

            public ResolvingScope(Registration regist)
            {
                _regist = regist;
                _regist.IsResolving = true;
            }

            public void Dispose()
            {
                _regist.IsResolving = false;
            }
        }

        public Registration()
        { }

        public Type Type { get; init; }
        public OnConstructObject Construct { get; init; }
        public bool IsInstance { get; init; }
        public Type AsType { get; init; }

        public object Instance { get; set; }
        public bool IsResolving { get; private set; }

        private Action<IRegistration, object> _onResolved;
        private Action<IRegistration, object> _onReleasing;
        private Action<IRegistration> _onReleased;
        private Action<IRegistration> _onUnRegisterType;

        public ResolvingScope NewResolvingScope() => new ResolvingScope(this);

        public object ConstructObject(Type target, object[] extraInfos, object[] args)
        {
            if (Construct == null) { return Activator.CreateInstance(Type, args); }
            var result = Construct(this, target, extraInfos, args);
            if (result == null) { throw new ContainerException($"未能正确创建对象：{target}"); }
            return result;
        }

        public IRegistration OnResolved(Action<IRegistration, object> callback)
        {
            _onResolved = callback;
            return this;
        }

        public IRegistration OnReleasing(Action<IRegistration, object> callback)
        {
            if (!IsInstance) { throw new ContainerException("仅实例注册可以设置释放回调"); }
            _onReleasing = callback;
            return this;
        }

        public IRegistration OnReleased(Action<IRegistration> callback)
        {
            if (!IsInstance) { throw new ContainerException("仅实例注册可以设置释放完成回调"); }
            _onReleased = callback;
            return this;
        }

        public IRegistration OnUnRegisterType(Action<IRegistration> callback)
        {
            _onUnRegisterType = callback;
            return this;
        }

        public void TriggerOnResolved(object instance)
        {
            _onResolved?.Invoke(this, instance);
        }

        public void TriggerOnReleasing()
        {
            _onReleasing?.Invoke(this, Instance);
        }

        public void TriggerOnReleased()
        {
            _onReleased?.Invoke(this);
        }

        public void TriggerOnUnRegisterType()
        {
            _onUnRegisterType?.Invoke(this);
        }
    }
}