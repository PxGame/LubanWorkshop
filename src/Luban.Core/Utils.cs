using Luban.Core.Containers;
using Luban.Core.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Luban.Core
{
    public static class Utils
    {
        private static AppEntry _entry;

        public static bool IsInited => _entry != null;
        public static IAppEntry Entry => _entry;
        public static IContainer Container => _entry?.Container;
        public static IServiceCollection Services => _entry?.Services;

        public static string AppName => "Luban";
        public static string AppFolder { get; }
        public static string UserFolder { get; }
        public static string AppLogFolder { get; }
        public static string AppPluginsFolder { get; }
        public static string UserPluginsFolder { get; }

        public static string AppSettingsFolder { get; }
        public static string UserSettingsFolder { get; }

        static Utils()
        {
            AppFolder = AppContext.BaseDirectory.StandardizedPath();
            UserFolder =
                PathCombine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    AppName);

            AppLogFolder = PathCombine(UserFolder, "Logs");

            AppPluginsFolder = PathCombine(AppFolder, "Plugins");
            UserPluginsFolder = PathCombine(UserFolder, "Plugins");

            AppSettingsFolder = PathCombine(AppFolder, "Settings");
            UserSettingsFolder = PathCombine(UserFolder, "Settings");
        }

        public static async Task Initialize()
        {
            if (_entry != null) { return; }
            var result = new AppEntry();
            await result.Initialize();
            _entry = result;
        }

        public static async Task Dispose()
        {
            if (_entry == null) { return; }
            await _entry.Shutdown();
            _entry.Dispose();
            _entry = null;
        }

        public static string PathCombine(params string[] paths)
        {
            return Path.Combine(paths).StandardizedPath();
        }

        public static string StandardizedPath(this string path)
        {
            if (string.IsNullOrEmpty(path)) { return path; }
            path = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return path;
        }
    }
}