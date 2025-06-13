using System;
using System.Reflection;

namespace LB.Core.Containers
{
    public delegate object OnConstructObject(IRegistration regist, Type type, object[] extraInfos, object[] args);

    public interface IRegistration
    {
        Type AsType { get; }
        OnConstructObject Construct { get; }
        bool IsInstance { get; }
        Type Type { get; }

        IRegistration OnResolved(Action<IRegistration, object> callback);

        IRegistration OnReleasing(Action<IRegistration, object> callback);

        IRegistration OnReleased(Action<IRegistration> callback);

        IRegistration OnUnRegisterType(Action<IRegistration> callback);
    }
}