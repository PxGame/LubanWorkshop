using LB.Core.Containers;
using LB.Core.Services.Logs;
using LB.Core.Services.Settings;
using System.Threading.Tasks;

namespace LB.Core.Services.Plugins
{
    public interface IPlugin : IOnResolved
    {
        IPluginConfig Config { get; init; }
        ILog Log { get; init; }
        string RootFolder { get; init; }

        Task OnLoad();

        Task OnUnload();
    }

    public abstract class IPlugin<T> : IPlugin where T : class
    {
        [Inject] public IServiceCollection services { get; init; }
        [Inject] public string RootFolder { get; init; }
        [Inject] public IPluginConfig Config { get; init; }
        [Inject] public ILog Log { get; init; }

        [CustomSetting("usersetting.json")]
        [Inject] public ICustomSetting<T> UserSetting { get; init; }

        public abstract void OnResolved();

        public abstract Task OnLoad();

        public virtual async Task OnUnload()
        {
            UserSetting.Save();
            await Task.CompletedTask;
        }
    }
}