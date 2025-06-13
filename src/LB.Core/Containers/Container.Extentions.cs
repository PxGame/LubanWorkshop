using System;

namespace LB.Core.Containers
{
    internal partial class Container
    {
        #region Resovle

        public T Resolve<T>(object[] extraInfos) where T : class
        {
            return Resolve(typeof(T), extraInfos, []) as T;
        }

        public T Resolve<T>(object[] extraInfos, object[] args) where T : class
        {
            return Resolve(typeof(T), extraInfos, args) as T;
        }

        #endregion Resovle

        #region Not Instance

        public IRegistration RegisterType<T>()
        {
            return RegisterType(typeof(T), null, false, null);
        }

        public IRegistration RegisterType<T>(OnConstructObject construct)
        {
            return RegisterType(typeof(T), construct != null ? (r, t, p, args) => construct(r, t, p, args) : null, false, null);
        }

        #endregion Not Instance

        #region Instance

        public IRegistration RegisterInstance<T, AsT>()
        {
            return RegisterInstance<T, AsT>(null);
        }

        public IRegistration RegisterInstance<T, AsT>(OnConstructObject construct)
        {
            return RegisterType(typeof(T), construct != null ? (r, t, p, args) => construct(r, t, p, args) : null, true, typeof(AsT));
        }

        public IRegistration RegisterInstance<T>()
        {
            return RegisterInstance<T>(null);
        }

        public IRegistration RegisterInstance<T>(OnConstructObject construct)
        {
            return RegisterType(typeof(T), construct != null ? (r, t, p, args) => construct(r, t, p, args) : null, true, null);
        }

        #endregion Instance
    }
}