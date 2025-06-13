using System;
using System.Reflection;

namespace LB.Core.Containers
{
    public interface IContainer : IDisposable
    {
        object Inject(object instance);

        bool IsRegistered(Type type);

        IRegistration RegisterInstance<T, AsT>();

        IRegistration RegisterInstance<T, AsT>(OnConstructObject construct);

        IRegistration RegisterInstance<T>();

        IRegistration RegisterInstance<T>(OnConstructObject construct);

        IRegistration RegisterType<T>();

        IRegistration RegisterType(Type type, OnConstructObject construct, bool isInstance, Type asType);

        IRegistration RegisterType<T>(OnConstructObject construct);

        object Resolve(Type type, object[] extraInfos, object[] args);

        T Resolve<T>(object[] extraInfos, object[] args) where T : class;

        T Resolve<T>(object[] extraInfos) where T : class;
    }
}