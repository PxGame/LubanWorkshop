using Luban.Services;

namespace Luban.Core.Services.Settings
{
    public abstract class ISettingService : IService
    {
        public abstract T Load<T>(string relativeFilePath, bool isAppFolder);

        public abstract void Save<T>(string relativeFilePath, T data, bool isAppFolder);
    }
}