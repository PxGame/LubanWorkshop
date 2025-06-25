using Luban.Core.Containers;
using System.Threading.Tasks;

namespace Luban.Services
{
    public interface IService : IOnResolved, IOnInstanceReleased
    {
        Task OnServiceInitialize();

        Task OnServiceShutdown();
    }
}