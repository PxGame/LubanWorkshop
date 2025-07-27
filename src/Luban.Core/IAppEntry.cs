using Luban.Core.Containers;
using Luban.Core.Services;
using Luban.Services;
using System;
using System.Threading.Tasks;

namespace Luban.Core
{
    public interface IAppEntry : IDisposable
    {
        IContainer Container { get; }

        IServiceCollection Services { get; }
    }
}