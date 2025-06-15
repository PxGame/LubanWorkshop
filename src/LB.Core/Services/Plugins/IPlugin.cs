using LB.Core.Containers;
using LB.Core.Services.Logs;
using LB.Core.Services.Settings;
using System;
using System.Collections.Generic;
using System.IO;
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

    [LogRoot(GetLogPropertyDictMethodName = "GetLogPropertyDict")]
    [CustomSettingRoot(SubPath = "Plugins/", GetNextSubPathMethodName = "GetCustomSettingNextSubPath")]
    public abstract class IPlugin<T> : IPlugin where T : class
    {
        [Inject] public IServiceCollection services { get; init; }
        [Inject] public string RootFolder { get; init; }
        [Inject] public IPluginConfig Config { get; init; }

        [Inject] public ILog Log { get; init; }

        [CustomSetting("setting.json")]
        [Inject] public ICustomSetting<T> UserSetting { get; init; }

        protected virtual string GetCustomSettingNextSubPath()
        {
            return Config.Name;
        }

        protected virtual Dictionary<string, object> GetLogPropertyDict()
        {
            var result = new Dictionary<string, object>();
            //var datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            result["RelativeFilePath"] = Path.Combine("Plugins", Config.Name, $"plugin_.log");
            return result;
        }

        public abstract void OnResolved();

        public abstract Task OnLoad();

        public virtual async Task OnUnload()
        {
            UserSetting.Save();
            await Task.CompletedTask;
        }
    }
}