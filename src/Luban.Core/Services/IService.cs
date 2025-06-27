using Luban.Core.Containers;
using Luban.Core.Services.Logs;
using System.Threading.Tasks;

namespace Luban.Services
{
    public abstract class IService : IOnResolved, IOnInstanceReleased
    {
        [Inject] protected IContainer Container { get; init; }
        protected ILog Log { get; private set; }

        public abstract void OnResolved();

        public abstract void OnInstanceReleased();

        public abstract Task OnServiceInitialing();

        public virtual async Task OnServiceInitialized()
        {
            Log = Container.Resolve<ILog>([new LogAttribute() { Tag = GetType().Name }]);
            await Task.CompletedTask;
        }

        public abstract Task OnServiceShutdown();
    }
}