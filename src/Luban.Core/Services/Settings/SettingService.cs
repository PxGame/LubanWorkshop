﻿using Luban.Core.Containers;
using Luban.Core.Services.Logs;
using Luban.Core.Services.Plugins;
using Luban.Core.Services.Storages;
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
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SettingAttribute : Attribute
    {
        public FileStorageType StorageType { get; }
        public string RelativePath { get; }
        public string RelativeRootPathMethodName { get; set; }

        public SettingAttribute(FileStorageType storageType, string relativePath)
        {
            StorageType = storageType;
            RelativePath = relativePath;
        }
    }

    public interface ISetting
    {
        void Set<T>(string key, T value);

        bool TryGetValue<T>(string key, out T value);

        void Load();

        void Save();
    }

    internal class Setting : ISetting
    {
        public ISettingService service { get; }
        public FileStorageType storageType { get; }
        public string relativePath { get; }

        private JObject _rawData;
        private bool isDirty;

        public Setting(ISettingService service, FileStorageType storageType, string relativePath)
        {
            this.service = service;
            this.storageType = storageType;
            this.relativePath = relativePath;
        }

        public void Load()
        {
            _rawData = service.Load(storageType, relativePath);
            _rawData.PropertyChanged += (sender, e) =>
            {
                isDirty = true;
            };
        }

        public void Save()
        {
            if (!isDirty) { return; }
            service.Save(storageType, relativePath, _rawData);
        }

        public void Set<T>(string key, T value)
        {
            _rawData[key] = JToken.FromObject(value);
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            var token = _rawData.SelectToken(key, false);
            if (token != null)
            {
                value = token.ToObject<T>();
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
    }

    internal class SettingService : ISettingService
    {
        private IStorageService storage { get; set; }

        public override void OnResolved()
        {
            Container.RegisterType(typeof(ISetting), OnCreateSetting, false, null, false);
        }

        private object OnCreateSetting(IRegistration regist, Type type, List<object> extraInfos, object[] args)
        {
            SettingAttribute settingInfo = extraInfos.FirstOrDefault(x => x is SettingAttribute) as SettingAttribute;

            if (settingInfo == null) { return null; }

            //var injectTarget = extraInfos.FirstOrDefault(x => x is InjectTarget) as InjectTarget;

            var setting = new Setting(this, settingInfo.StorageType, settingInfo.RelativePath);
            setting.Load();

            return setting;
        }

        public override void OnInstanceReleased()
        {
            Log.Information($"OnInstanceReleased");
        }

        public override async Task OnServiceInitialing()
        {
            await base.OnServiceInitialing();

            storage = Container.Resolve<IStorageService>();

            MainSetting = Container.Resolve<ISetting>([new SettingAttribute(FileStorageType.AppFolder, "setting.json")]);
            UserSetting = Container.Resolve<ISetting>([new SettingAttribute(FileStorageType.UserFolder, "setting.json")]);

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

        internal override JObject Load(FileStorageType storageType, string relativePath)
        {
            var jsonStr = storage.ReadFileText(storageType, relativePath);
            jsonStr.Wait();
            if (string.IsNullOrEmpty(jsonStr.Result)) { return new JObject(); }
            var jsonObj = JObject.Parse(jsonStr.Result);
            return jsonObj;
        }

        internal override void Save(FileStorageType storageType, string relativePath, JObject setting)
        {
            var jsonStr = setting.ToString(Formatting.Indented);
            storage.WriteFileText(storageType, relativePath, jsonStr).Wait();
        }
    }
}