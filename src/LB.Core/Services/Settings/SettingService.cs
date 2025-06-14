using LB.Core.Containers;
using LB.Core.Services.Logs;
using LB.Core.Services.Plugins;
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

namespace LB.Core.Services.Settings
{
    internal class SettingService : ISettingService
    {
        [Inject]
        [LogInfo(Tag = "设置服务")]
        private ILog Log { get; init; }

        [Inject]
        private IContainer Container { get; init; }

        public string SettingPath { get; private set; }

        [CustomSetting("setting")]
        public ICustomSetting<MainSetting> MainSetting { get; private set; }

        [CustomSetting("setting")]
        public ICustomSetting<UserSetting> UserSetting { get; private set; }

        public void OnResolved()
        {
            Container.RegisterType(typeof(ICustomSetting<>), OnCreateSetting, false, null);

            Log.Information($"\nAppFolder : {Utils.AppFolder}\nAppDataFolder : {Utils.AppDataFolder}");
        }

        private object OnCreateSetting(IRegistration regist, Type type, object[] extraInfos, object[] args)
        {
            CustomSettingAttribute settingInfo = extraInfos.FirstOrDefault(x => x is CustomSettingAttribute) as CustomSettingAttribute;

            if (settingInfo != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICustomSetting<>))
            {
                var settingType = type.GetGenericArguments()[0];
                var settingInstance = Activator.CreateInstance(typeof(CustomSetting<>).MakeGenericType(settingType), settingInfo.RelativePath, settingInfo.IsAppFolder);
                return settingInstance;
            }

            return null;
        }

        public void OnInstanceReleased()
        {
            Log.Information($"OnInstanceReleased");
        }

        public async Task OnServiceInitialize()
        {
            Log.Information($"OnServiceInitialize");

            MainSetting = Container.Resolve<ICustomSetting<MainSetting>>([new CustomSettingAttribute("setting.json") { IsAppFolder = true }]);
            UserSetting = Container.Resolve<ICustomSetting<UserSetting>>([new CustomSettingAttribute("setting.json")]);

            await Task.CompletedTask;
        }

        public async Task OnServiceShutdown()
        {
            Log.Information($"OnServiceShutdown");

            UserSetting.Save();

            await Task.CompletedTask;
        }

        public T Load<T>(string relativeFilePath, bool isAppFolder)
        {
            var fullPath = Path.Combine(isAppFolder ? Utils.AppFolder : Utils.AppDataFolder, relativeFilePath);
            try
            {
                var jsonData = File.ReadAllText(fullPath);
                return JsonConvert.DeserializeObject<T>(jsonData);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"配置文件解析失败, 返回默认值: {fullPath}");
                return Activator.CreateInstance<T>();
            }
        }

        public void Save<T>(string relativeFilePath, T data, bool isAppFolder)
        {
            var fullPath = Path.Combine(isAppFolder ? Utils.AppFolder : Utils.AppDataFolder, relativeFilePath);
            try
            {
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