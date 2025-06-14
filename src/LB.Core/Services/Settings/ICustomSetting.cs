using LB.Core.Containers;
using System;

namespace LB.Core.Services.Settings
{
    public class SaveScope : IDisposable
    {
        private ICustomSetting _setting;

        public SaveScope(ICustomSetting setting)
        {
            this._setting = setting;
        }

        public void Dispose()
        {
            if (_setting != null)
            {
                _setting.Save();
            }
        }
    }

    public interface ICustomSetting : IOnResolved
    {
        bool IsAppFolder { get; init; }
        Action OnReload { get; set; }
        Action OnSaved { get; set; }
        string RelativeFilePath { get; init; }

        void Reload();

        void Save();

        SaveScope NewSaveScope();
    }

    public interface ICustomSetting<T> : ICustomSetting
    {
        T Data { get; }
    }
}