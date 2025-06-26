using Luban.Core.Containers;
using System;

namespace Luban.Core
{
    public interface IAppEntry : IDisposable
    {
        IContainer Container { get; }
    }
}