using System;
using System.IO;

namespace Luban.Core
{
    public static class Utils
    {
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