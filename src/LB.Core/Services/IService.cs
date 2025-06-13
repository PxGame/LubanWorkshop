using LB.Core.Containers;
using System.Threading.Tasks;

namespace LB.Services
{
    public interface IService : IOnResolved, IOnInstanceReleased
    {
        Task OnServiceInitialize();

        Task OnServiceShutdown();
    }
}