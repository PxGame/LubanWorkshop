using Luban.Services;

namespace Luban.Core.Services.Settings
{
    public abstract class ISettingService : IService
    {
        public abstract ISetting Load(string rootRelativePath, string subFilePath, SettingType settingType);

        public abstract void Save(ISetting setting);
    }
}