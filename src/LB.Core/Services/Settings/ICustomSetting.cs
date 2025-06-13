using System;

namespace LB.Core.Services.Settings
{
    public interface ICustomSetting
    {
        string RelativePath { get; }

        Action OnReload { get; set; }
        Action OnSaved { get; set; }

        void Reload();

        void Save();
    }
}