using Luban.Core.Containers;
using System.Threading.Tasks;

namespace Luban.Services
{
    public abstract class IService : IOnResolved, IOnInstanceReleased
    {
        [Inject] protected IContainer Container { get; init; }

        public abstract void OnResolved();

        public abstract void OnInstanceReleased();

        public abstract Task OnServiceInitialing();

        public abstract Task OnServiceInitialized();

        public abstract Task OnServiceShutdown();
    }
}