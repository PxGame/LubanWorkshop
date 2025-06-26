using System;
using System.IO;

namespace Luban.Core
{
    public static class Utils
    {
        public static string AppName => "Luban";
        public static string AppFolder { get; }
        public static string AppDataFolder { get; }
        public static string AppLogFolder { get; }
        public static string AppPluginsFolder { get; }
        public static string AppDataPluginsFolder { get; }
        public static string AppDataSettingsFolder { get; }

        static Utils()
        {
            AppFolder = AppContext.BaseDirectory.StandardizedPath();
            AppDataFolder =
                Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                AppName).StandardizedPath();

            AppLogFolder = Path.Combine(AppDataFolder, "Logs").StandardizedPath();

            AppPluginsFolder = Path.Combine(AppFolder, "Plugins").StandardizedPath();
            AppDataPluginsFolder = Path.Combine(AppDataFolder, "Plugins").StandardizedPath();

            AppDataSettingsFolder = Path.Combine(AppDataFolder, "Settings").StandardizedPath();
        }

        public static string StandardizedPath(this string path)
        {
            if (string.IsNullOrEmpty(path)) { return path; }
            path = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return path;
        }
    }
}