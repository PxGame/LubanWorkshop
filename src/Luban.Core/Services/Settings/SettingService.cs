using Luban.Core.Containers;
using Luban.Core.Services.Logs;
using Luban.Core.Services.Plugins;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Luban.Core.Services.Settings
{
    internal class SettingService : ISettingService
    {
        private ILog Log { get; set; }

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
                var rootSubPath = settingRoot.GetNextSubPath(injectTarget.Target);
                settingPath = Path.Combine(settingPath, rootSubPath).StandardizedPath();
            }

            if (!string.IsNullOrEmpty(settingInfo.RelativePath))
            {
                settingPath = Path.Combine(settingPath, settingInfo.RelativePath).StandardizedPath();
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
            Log = Container.Resolve<ILog>([new LogAttribute() { Tag = "设置服务" }]);
            Log.Information($"OnServiceInitialized");
            Log.Information($"\nAppFolder : {Utils.AppFolder}\nAppDataFolder : {Utils.AppDataFolder}");

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
            var fullPath = Path.Combine(isAppFolder ? Utils.AppFolder : Utils.AppDataSettingsFolder, relativeFilePath).StandardizedPath();
            return fullPath;
        }

        public override T Load<T>(string relativeFilePath, bool isAppFolder)
        {
            var fullPath = GetFullPath(relativeFilePath, isAppFolder);
            try
            {
                var jsonData = File.ReadAllText(fullPath);
                return JsonConvert.DeserializeObject<T>(jsonData);
            }
            catch (Exception)
            {
                return Activator.CreateInstance<T>();
            }
        }

        public override void Save<T>(string relativeFilePath, T data, bool isAppFolder)
        {
            var fullPath = GetFullPath(relativeFilePath, isAppFolder);
            try
            {
                var folder = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(fullPath, jsonData);
                Log.Information($"配置文件已保存: {fullPath}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"配置文件保存失败: {fullPath}");
            }
        }
    }
}