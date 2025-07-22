using Luban.Core.Containers;
using Luban.Core.Services.Logs;
using Luban.Core.Services.Settings;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Luban.Core.Services.Plugins
{
    public abstract class IPlugin : IOnResolved
    {
        [Inject] public IServiceCollection services { get; init; }
        [Inject] public string RootFolder { get; init; }

        [Inject] public IPluginConfig Config { get; init; }

        [Log(Tag = "Plugin", CustomPropertyMethodName = nameof(GetLogCustomProperty))]
        [Inject] public ILog Log { get; init; }

        [Setting(Storages.FileStorageType.UserFolder, "setting.json", RelativeRootPathMethodName = nameof(GetSettingRelativeRootPath))]
        [Inject] public ISetting UserSetting { get; init; }

        protected virtual string GetSettingRelativeRootPath()
        {
            return Utils.PathCombine("Plugins", Config.Name);
        }

        protected virtual Dictionary<string, object> GetLogCustomProperty()
        {
            var result = new Dictionary<string, object>();
            //var datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            result["RelativeFilePath"] = Utils.PathCombine("Plugins", Config.Name, $"plugin_.log");
            result["LogTag"] = Config.Name;
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