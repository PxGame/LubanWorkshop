﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Luban.Core.Containers
{
    public interface IContainer : IDisposable
    {
        object Inject(object instance, InjectExtraPropertyValue extraPropertyValue);

        bool IsRegistered(Type type);

        IRegistration RegisterInstance<T, AsT>();

        IRegistration RegisterInstance<T, AsT>(OnConstructObject construct);

        IRegistration RegisterInstance<T>();

        IRegistration RegisterInstance<T>(OnConstructObject construct);

        IRegistration RegisterType<T>();

        IRegistration RegisterType(Type type, OnConstructObject construct, bool isInstance, Type asType, bool supportAssignableType);

        IRegistration RegisterType<T>(OnConstructObject construct);

        object Resolve(Type type, List<object> extraInfos, object[] args, InjectExtraPropertyValue extraPropertyValue);

        T Resolve<T>(List<object> extraInfos, object[] args) where T : class;

        T Resolve<T>(List<object> extraInfos) where T : class;

        T Resolve<T>() where T : class;
    }
}