﻿using Luban.Core.Services.Logs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Core.Services.Storages
{
    public enum FileStorageType
    {
        AppFolder,
        UserFolder,
        RemoteFolder,
    }

    internal class StorageService : IStorageService
    {
        public override void OnResolved()
        {
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
            await Task.CompletedTask;
        }

        public override async Task OnServiceShutdown()
        {
            Log.Information($"OnServiceShutdown");
            await Task.CompletedTask;
        }

        public override string ReadFileText(FileStorageType storageType, string relativeFilePath)
        {
            try
            {
                switch (storageType)
                {
                    case FileStorageType.AppFolder:
                        {
                            var fullPath = Utils.PathCombine(Utils.AppFolder, relativeFilePath);
                            if (!File.Exists(fullPath)) { throw new FileNotFoundException($"File not found: {fullPath}"); }
                            return File.ReadAllText(fullPath);
                        }

                    case FileStorageType.UserFolder:
                        {
                            var fullPath = Utils.PathCombine(Utils.UserFolder, relativeFilePath);
                            if (!File.Exists(fullPath)) { throw new FileNotFoundException($"File not found: {fullPath}"); }
                            return File.ReadAllText(fullPath);
                        }

                    case FileStorageType.RemoteFolder:
                        // Remote storage handling is not implemented in this example.
                        throw new NotImplementedException("Remote storage handling is not implemented yet.");

                    default:
                        throw new NotSupportedException($"Unsupported storage type: {storageType}");
                }
            }
            catch (Exception)
            {
                return string.Empty; // Return empty string if an error occurs, could also log the error.
            }
        }

        public override void WriteFileText(FileStorageType storageType, string relativeFilePath, string content)
        {
            switch (storageType)
            {
                case FileStorageType.AppFolder:
                    {
                        var fullPath = Utils.PathCombine(Utils.AppFolder, relativeFilePath);
                        File.WriteAllText(fullPath, content);
                    }
                    break;

                case FileStorageType.UserFolder:
                    {
                        var fullPath = Utils.PathCombine(Utils.UserFolder, relativeFilePath);
                        File.WriteAllText(fullPath, content);
                    }
                    break;

                case FileStorageType.RemoteFolder:
                    // Remote storage handling is not implemented in this example.
                    throw new NotImplementedException("Remote storage handling is not implemented yet.");

                default:
                    throw new NotSupportedException($"Unsupported storage type: {storageType}");
            }
        }
    }
}