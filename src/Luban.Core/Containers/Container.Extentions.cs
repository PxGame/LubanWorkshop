using System;
using System.Collections.Generic;

namespace Luban.Core.Containers
{
    internal partial class Container
    {
        #region Resovle

        public T Resolve<T>() where T : class
        {
            return Resolve(typeof(T), [], [], null) as T;
        }

        public T Resolve<T>(List<object> extraInfos) where T : class
        {
            return Resolve(typeof(T), extraInfos, [], null) as T;
        }

        public T Resolve<T>(List<object> extraInfos, object[] args) where T : class
        {
            return Resolve(typeof(T), extraInfos, args, null) as T;
        }

        #endregion Resovle

        #region Not Instance

        public IRegistration RegisterType<T>()
        {
            return RegisterType(typeof(T), null, false, null, false);
        }

        public IRegistration RegisterType<T>(OnConstructObject construct)
        {
            return RegisterType(typeof(T), construct != null ? (r, t, p, args) => construct(r, t, p, args) : null, false, null, false);
        }

        #endregion Not Instance

        #region Instance

        public IRegistration RegisterInstance<T, AsT>()
        {
            return RegisterInstance<T, AsT>(null);
        }

        public IRegistration RegisterInstance<T, AsT>(OnConstructObject construct)
        {
            return RegisterType(typeof(T), construct != null ? (r, t, p, args) => construct(r, t, p, args) : null, true, typeof(AsT), false);
        }

        public IRegistration RegisterInstance<T>()
        {
            return RegisterInstance<T>(null);
        }

        public IRegistration RegisterInstance<T>(OnConstructObject construct)
        {
            return RegisterType(typeof(T), construct != null ? (r, t, p, args) => construct(r, t, p, args) : null, true, null, false);
        }

        #endregion Instance
    }
}