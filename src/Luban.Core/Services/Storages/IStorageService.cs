using Luban.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Core.Services.Storages
{
    public abstract class IStorageService : IService
    {
        public abstract Task<bool> WriteFileText(FileStorageType storageType, string relativeFilePath, string content);

        public abstract Task<string> ReadFileText(FileStorageType storageType, string relativeFilePath);
    }
}