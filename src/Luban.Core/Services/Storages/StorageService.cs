using FluentFTP;
using Luban.Core.Services.Settings;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Luban.Core.Services.Storages
{
    public enum FileStorageType
    {
        AppFolder,
        UserFolder,
        RemoteFolder,
    }

    internal class FtpSetting
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    internal class StorageService : IStorageService
    {
        private ISettingService setting { get; set; }

        public override void OnResolved()
        {
        }

        public override void OnInstanceReleased()
        {
            Log.Information($"OnInstanceReleased");
        }

        public override async Task OnServiceInitialing()
        {
            await base.OnServiceInitialing();
            setting = Container.Resolve<ISettingService>();

            await Task.CompletedTask;
        }

        public override async Task OnServiceInitialized()
        {
            await base.OnServiceInitialized();
            Log.Information($"OnServiceInitialized");
            await Task.CompletedTask;
        }

        public override async Task OnServiceShutdown()
        {
            Log.Information($"OnServiceShutdown");
            await Task.CompletedTask;
        }

        public override async Task<string> ReadFileText(FileStorageType storageType, string relativeFilePath)
        {
            try
            {
                switch (storageType)
                {
                    case FileStorageType.AppFolder:
                        {
                            var fullPath = Utils.PathCombine(Utils.AppFolder, relativeFilePath);
                            if (!File.Exists(fullPath)) { throw new FileNotFoundException($"File not found: {fullPath}"); }
                            return await File.ReadAllTextAsync(fullPath);
                        }

                    case FileStorageType.UserFolder:
                        {
                            var fullPath = Utils.PathCombine(Utils.UserFolder, relativeFilePath);
                            if (!File.Exists(fullPath)) { throw new FileNotFoundException($"File not found: {fullPath}"); }
                            return await File.ReadAllTextAsync(fullPath);
                        }

                    case FileStorageType.RemoteFolder:
                        {
                            throw new NotImplementedException("Remote storage handling is not implemented yet.");
                        }
                    default:
                        throw new NotSupportedException($"Unsupported storage type: {storageType}");
                }
            }
            catch (Exception)
            {
                return string.Empty; // Return empty string if an error occurs, could also log the error.
            }
        }

        public override async Task<bool> WriteFileText(FileStorageType storageType, string relativeFilePath, string content)
        {
            switch (storageType)
            {
                case FileStorageType.AppFolder:
                    {
                        var fullPath = Utils.PathCombine(Utils.AppFolder, relativeFilePath);
                        await File.WriteAllTextAsync(fullPath, content);
                    }
                    break;

                case FileStorageType.UserFolder:
                    {
                        var fullPath = Utils.PathCombine(Utils.UserFolder, relativeFilePath);
                        await File.WriteAllTextAsync(fullPath, content);
                    }
                    break;

                case FileStorageType.RemoteFolder:
                    // Remote storage handling is not implemented in this example.
                    throw new NotImplementedException("Remote storage handling is not implemented yet.");

                default:
                    throw new NotSupportedException($"Unsupported storage type: {storageType}");
            }

            return true;
        }
    }
}