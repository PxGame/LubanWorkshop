using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace LB.Core.Containers
{
    internal partial class Container : IContainer
    {
        private List<Registration> Registrations { get; init; } = new List<Registration>();

        private bool _disposedValue = false;

        public Action<IRegistration, object> OnResolved { get; set; }
        public Action<IRegistration, object> OnReleasing { get; set; }
        public Action<IRegistration> OnReleased { get; set; }
        public Action<IRegistration> OnUnRegisterType { get; set; }
        public Action<IRegistration> OnRegisterType { get; set; }

        private void TriggerOnResolved(Registration regist, object instance)
        {
            if (instance is IOnResolved onResolved) { onResolved.OnResolved(); }
            regist.TriggerOnResolved(instance);
            OnResolved?.Invoke(regist, instance);

            Debug.WriteLine($"[Container] 解析完成: {regist.Type}");
        }

        private void TriggerOnReleasing(Registration regist, object instance)
        {
            regist.TriggerOnReleasing();
            OnReleasing?.Invoke(regist, instance);

            Debug.WriteLine($"[Container] 释放开始: {regist.Type}");
        }

        private void TriggerOnReleased(Registration regist, object instance)
        {
            if (instance is IOnInstanceReleased onReleased) { onReleased.OnInstanceReleased(); }
            regist.TriggerOnReleased();
            OnReleased?.Invoke(regist);

            Debug.WriteLine($"[Container] 释放完成: {regist.Type}");
        }

        private void TriggerOnUnRegisterType(Registration regist)
        {
            regist.TriggerOnUnRegisterType();
            OnUnRegisterType?.Invoke(regist);

            Debug.WriteLine($"[Container] 注销类型: {regist.Type} (别名: {regist.AsType})");
        }

        private void TriggerOnRegisterType(Registration regist)
        {
            OnRegisterType?.Invoke(regist);

            Debug.WriteLine($"[Container] 注册类型: {regist.Type} (别名: {regist.AsType})");
        }

        public Container()
        {
            //注册当前实例
            RegisterType(typeof(Container), (_, _, _, _) => this, true, typeof(IContainer), false);
        }

        public IRegistration RegisterType(Type type, OnConstructObject construct, bool isInstance, Type asType, bool supportAssignableType)
        {
            if (type == null) { throw new ContainerException("类型不能为空"); }
            if (IsRegistered(type)) { throw new ContainerException($"类型已经被注册：{type} "); }
            if (asType != null && !asType.IsAssignableFrom(type)) { throw new ContainerException($"类型无法转换到别名类型：{type} => {asType} "); }

            var target = new Registration()
            {
                Type = type,
                Construct = construct,
                IsInstance = isInstance,
                AsType = asType,
                supportAssignableType = supportAssignableType,
            };
            Registrations.Add(target);

            TriggerOnRegisterType(target);

            return target;
        }

        public void UnRegisterType(Type type)
        {
            if (!IsRegistered(type)) { return; }
            var regist = GetRegistration(type);
            if (regist == null) { throw new ContainerException($"未注册类型: {type}"); }

            if (regist.IsInstance)
            {
                ReleaseInstance(regist);
            }

            TriggerOnUnRegisterType(regist);

            Registrations.Remove(regist);
        }

        public bool IsRegistered(Type type)
        {
            if (type == null) { throw new ContainerException("类型不能为空"); }
            return GetRegistration(type) != null;
        }

        internal object Resolve(Registration regist, Type targetType, List<object> extraInfos, object[] args, InjectExtraPropertyValue extraPropertyValue)
        {
            if (regist == null) { throw new ContainerException($"regist参数不能为空"); }
            if (regist.IsResolving) { throw new ContainerException($"循环依赖: {regist.Type}"); }

            try
            {
                object result = null;
                using (regist.NewResolvingScope())
                {
                    if (regist.IsInstance && regist.Instance != null) { return regist.Instance; }

                    result = regist.ConstructObject(targetType, extraInfos ?? [], args ?? []);
                    Inject(result, extraPropertyValue);

                    if (regist.IsInstance) { regist.Instance = result; }
                }
                TriggerOnResolved(regist, result);

                return result;
            }
            catch (Exception ex)
            {
                throw new ContainerException($"解析类型失败: {regist.Type}", ex);
            }
        }

        internal void ReleaseInstance(Registration regist)
        {
            if (regist == null) { throw new ContainerException("regist参数不能为空"); }
            if (!regist.IsInstance) { throw new Exception($"类型不是实例类型: {regist.Type}"); }

            var instance = regist.Instance;
            if (instance == null) { return; }
            TriggerOnReleasing(regist, instance);
            regist.Instance = null;
            TriggerOnReleased(regist, instance);
        }

        public void ReleaseInstance(Type type)
        {
            if (type == null) { throw new ContainerException("类型不能为空"); }
            var regist = GetRegistration(type);
            if (regist == null) { throw new ContainerException($"未注册类型: {type}"); }
            ReleaseInstance(regist);
        }

        public object Resolve(Type type, List<object> extraInfos, object[] args, InjectExtraPropertyValue extraPropertyValue)
        {
            if (type == null) { throw new ContainerException("类型不能为空"); }
            var regist = GetRegistration(type);
            if (regist == null) { throw new ContainerException($"未注册类型: {type}"); }
            return Resolve(regist, type, extraInfos, args, extraPropertyValue);
        }

        public object Inject(object instance, InjectExtraPropertyValue extraPropertyValue)
        {
            if (instance == null) { throw new ContainerException("实例不能为空"); }
            var type = instance.GetType();

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var prop in properties)
            {
                var inject = prop.GetCustomAttribute<InjectAttribute>(true);
                if (inject == null) { continue; }
                if (prop.GetValue(instance) != null) { continue; }

                if (extraPropertyValue != null && extraPropertyValue.TryGetValue(prop.Name, out object extraValue) && extraValue != null)
                {
                    var extraValueType = extraValue.GetType();
                    if (prop.PropertyType != extraValueType && !prop.PropertyType.IsAssignableFrom(extraValueType)) { throw new ContainerException($"属性 {prop.Name} 的类型 {prop.PropertyType} 无法转换为额外注入值的类型 {extraValue.GetType()}"); }
                    prop.SetValue(instance, extraValue);
                    continue;
                }

                if (inject.fromType != null && !prop.PropertyType.IsAssignableFrom(inject.fromType)) { throw new ContainerException($"属性 {prop.Name} 的类型 {prop.PropertyType} 无法转换为注入类型 {inject.fromType}"); }
                var propType = inject.fromType ?? prop.PropertyType;
                var extraInfos = prop.GetCustomAttributes(true).ToList();
                extraInfos.Add(new InjectTarget() { Target = instance });
                var propValue = Resolve(propType, extraInfos, [], null);
                prop.SetValue(instance, propValue);
            }

            return instance;
        }

        private bool CheckRegistType(Type type, Type registType, bool supportAssignableType)
        {
            if (type == null || registType == null) { return false; }
            if (type == registType) { return true; }
            if (supportAssignableType && registType.IsAssignableFrom(type)) { return true; }

            if (type.IsGenericType)
            {
                var gType = type.GetGenericTypeDefinition();
                if (registType.IsGenericTypeDefinition && registType == gType)
                {
                    return true;
                }
            }

            return false;
        }

        private Registration GetRegistration(Type type)
        {
            if (type == null) { throw new ContainerException("类型不能为空"); }
            foreach (var regist in Registrations)
            {
                if (CheckRegistType(type, regist.Type, regist.supportAssignableType)
                    || CheckRegistType(type, regist.AsType, regist.supportAssignableType))
                {
                    return regist;
                }
            }

            return null;
        }

        #region Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                var instanceRegists = Registrations.Where(t => t.IsInstance).Reverse();
                foreach (var regist in instanceRegists)
                {
                    ReleaseInstance(regist);
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion Dispose
    }
}