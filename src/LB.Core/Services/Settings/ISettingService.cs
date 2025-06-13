using LB.Services;

namespace LB.Core.Services.Settings
{
    public interface ISettingService : IService
    {
        T Load<T>(string relativeFilePath, bool isAppFolder);

        void Save<T>(string relativeFilePath, T data, bool isAppFolder);
    }
}