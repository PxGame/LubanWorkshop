using Luban.Core.Containers;
using System;

namespace Luban.Core.Services.Settings
{
    public class CustomSetting<T> : ICustomSetting<T>
    {
        [Inject]
        public ISettingService Setting { get; init; }

        public bool IsAppFolder { get; init; }
        public string RelativeFilePath { get; init; }

        public Action OnReload { get; set; }
        public Action OnSaved { get; set; }
        public T Data { get; private set; }

        public CustomSetting(string relativePath, bool isAppFolder)
        {
            RelativeFilePath = relativePath;
            IsAppFolder = isAppFolder;
        }

        public virtual SaveScope NewSaveScope() => new SaveScope(this);

        public virtual void OnResolved()
        {
            Reload();
        }

        public virtual void Reload()
        {
            Data = Setting.Load<T>(RelativeFilePath, IsAppFolder);
            OnReload?.Invoke();
        }

        public virtual void Save()
        {
            Setting.Save<T>(RelativeFilePath, Data, IsAppFolder);
            OnSaved?.Invoke();
        }
    }
}