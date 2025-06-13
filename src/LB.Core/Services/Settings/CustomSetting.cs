using LB.Core.Containers;
using System;

namespace LB.Core.Services.Settings
{
    public interface ICustomSetting<T> : IOnResolved
    {
        T Data { get; }

        void Reload();

        void Save();
    }

    internal class CustomSetting<T> : ICustomSetting<T>
    {
        [Inject]
        public ISettingService Setting { get; init; }

        public Action OnReload { get; set; }
        public Action OnSaved { get; set; }
        public T Data { get; private set; }

        public string RelativeFilePath { get; }
        public bool IsAppFolder { get; }

        public CustomSetting(string relativePath, bool isAppFolder)
        {
            RelativeFilePath = relativePath;
            IsAppFolder = isAppFolder;
        }

        public void Reload()
        {
            Data = Setting.Load<T>(RelativeFilePath, IsAppFolder);
            OnReload?.Invoke();
        }

        public void Save()
        {
            Setting.Save<T>(RelativeFilePath, Data, IsAppFolder);
            OnSaved?.Invoke();
        }

        public void OnResolved()
        {
            Reload();
        }
    }
}