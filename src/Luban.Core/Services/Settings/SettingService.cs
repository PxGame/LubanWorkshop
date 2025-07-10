using Luban.Core.Containers;
using Luban.Core.Services.Logs;
using Luban.Core.Services.Plugins;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Core.Services.Settings
{
    public enum SettingType
    {
        Main,
        LocalUser,
        MainRemote,
    }

    public interface ISetting
    {
        SettingType settingType { get; }
    }

    internal class Setting : ISetting
    {
        public SettingType settingType { get; }

        public Setting(ISettingService setting, string rootRelativePath, string fileRelativePath, SettingType settingType)
        {
            this.settingType = settingType;
        }
    }

    internal class SettingService : ISettingService
    {
        public string SettingPath { get; private set; }

        public ICustomSetting<MainSetting> MainSetting { get; private set; }

        public ICustomSetting<UserSetting> UserSetting { get; private set; }

        public override void OnResolved()
        {
            Container.RegisterType(typeof(ICustomSetting<>), OnCreateSetting, false, null, false);

            MainSetting = Container.Resolve<ICustomSetting<MainSetting>>([new CustomSettingAttribute("setting.json") { IsAppFolder = true }]);
            UserSetting = Container.Resolve<ICustomSetting<UserSetting>>([new CustomSettingAttribute("setting.json") { }]);
        }

        private object OnCreateSetting(IRegistration regist, Type type, List<object> extraInfos, object[] args)
        {
            CustomSettingAttribute settingInfo = extraInfos.FirstOrDefault(x => x is CustomSettingAttribute) as CustomSettingAttribute;

            if (settingInfo == null || !type.IsGenericType || type.GetGenericTypeDefinition() != typeof(ICustomSetting<>)) { return null; }

            var injectTarget = extraInfos.FirstOrDefault(x => x is InjectTarget) as InjectTarget;
            var settingPath = string.Empty;
            if (injectTarget != null && injectTarget.Target != null)
            {
                var settingRoot = injectTarget.Target.GetType().GetCustomAttribute<CustomSettingRootAttribute>(true);
                if (settingRoot != null)
                {
                    var rootSubPath = settingRoot.GetNextSubPath(injectTarget.Target);
                    settingPath = Utils.PathCombine(settingPath, rootSubPath);
                }
            }

            if (!string.IsNullOrEmpty(settingInfo.RelativePath))
            {
                settingPath = Utils.PathCombine(settingPath, settingInfo.RelativePath);
            }

            var settingType = type.GetGenericArguments()[0];
            var settingInstance = Activator.CreateInstance(typeof(CustomSetting<>).MakeGenericType(settingType), this, settingPath, settingInfo.IsAppFolder);
            return settingInstance;
        }

        public override void OnInstanceReleased()
        {
            Log.Information($"OnInstanceReleased");
        }

        public override async Task OnServiceInitialing()
        {
            await Task.CompletedTask;
        }

        public override async Task OnServiceInitialized()
        {
            await base.OnServiceInitialized();
            Log.Information($"OnServiceInitialized");
            Log.Information($"\nAppFolder : {Utils.AppFolder}\nAppDataFolder : {Utils.UserFolder}");

            await Task.CompletedTask;
        }

        public override async Task OnServiceShutdown()
        {
            Log.Information($"OnServiceShutdown");

            UserSetting.Save();

            await Task.CompletedTask;
        }

        protected string GetFullPath(string relativeFilePath, bool isAppFolder)
        {
            var fullPath = Utils.PathCombine(isAppFolder ? Utils.AppFolder : Utils.UserSettingsFolder, relativeFilePath);
            return fullPath;
        }

        public override ISetting Load(string rootRelativePath, string subFilePath, SettingType settingType)
        {
            string data = string.Empty;
            switch (settingType)
            {
                case SettingType.Main:
                    data = LoadMain(rootRelativePath, subFilePath);
                    break;

                case SettingType.LocalUser:
                    data = LoadLocalUser(rootRelativePath, subFilePath);
                    break;

                case SettingType.MainRemote:
                    data = LoadMainRemote(rootRelativePath, subFilePath);
                    break;

                default:
                    throw new ArgumentException($"Unsupported setting type: {settingType}");
            }

            var jsonNode = JObject.Parse(data);
            throw new ArgumentException($"Unsupported setting type");
        }

        public override void Save(ISetting setting)
        {
            switch (setting.settingType)
            {
                case SettingType.Main:
                    SaveMain(setting);
                    break;

                case SettingType.LocalUser:
                    SaveLocalUser(setting);
                    break;

                case SettingType.MainRemote:
                    SaveMainRemote(setting);
                    break;

                default:
                    throw new ArgumentException($"Unsupported setting type: {setting.settingType}");
            }
        }

        private void SaveMainRemote(ISetting setting)
        {
            throw new NotImplementedException();
        }

        private void SaveLocalUser(ISetting setting)
        {
            throw new NotImplementedException();
        }

        private void SaveMain(ISetting setting)
        {
            throw new NotImplementedException();
        }

        private string LoadMainRemote(string rootRelativePath, string subFilePath)
        {
            throw new NotImplementedException();
        }

        private string LoadLocalUser(string rootRelativePath, string subFilePath)
        {
            var fullPath = Utils.PathCombine(Utils.UserSettingsFolder, rootRelativePath, subFilePath);
            throw new NotImplementedException();
        }

        private string LoadMain(string rootRelativePath, string subFilePath)
        {
            var fullPath = Utils.PathCombine(Utils.AppSettingsFolder, rootRelativePath, subFilePath);

            using var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            throw new NotImplementedException();
        }
    }
}