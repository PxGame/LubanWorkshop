using System;
using System.IO;
using System.Reflection;

namespace LB.Core
{
    public static class Utils
    {
        public static string AppName => "LubanWorkshop";
        public static string AppFolder { get; }
        public static string AppDataFolder { get; }

        static Utils()
        {
            AppFolder = AppContext.BaseDirectory.StandardizedPath();
            AppDataFolder =
                Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                AppName).StandardizedPath();
        }

        public static string StandardizedPath(this string path)
        {
            if (string.IsNullOrEmpty(path)) { return path; }
            path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            return path;
        }
    }
}