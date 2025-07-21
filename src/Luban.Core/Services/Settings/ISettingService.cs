using Luban.Core.Services.Storages;
using Luban.Services;
using Newtonsoft.Json.Linq;

namespace Luban.Core.Services.Settings
{
    public abstract class ISettingService : IService
    {
        public abstract JObject Load(FileStorageType storageType, string relativePath);

        public abstract void Save(FileStorageType storageType, string relativePath, JObject setting);
    }
}