using Luban.Core.Services.Storages;
using Luban.Services;
using Newtonsoft.Json.Linq;

namespace Luban.Core.Services.Settings
{
    public abstract class ISettingService : IService
    {
        public ISetting MainSetting { get; protected set; }

        public ISetting UserSetting { get; protected set; }

        internal abstract JObject Load(FileStorageType storageType, string relativePath);

        internal abstract void Save(FileStorageType storageType, string relativePath, JObject setting);
    }
}