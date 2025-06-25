using Luban.Core.Services.Plugins;
using System;
using System.Collections.Generic;

namespace Luban.Plugin
{
    /// <summary>
    /// 参考 Unity Package
    /// https://docs.unity.cn/Manual/upm-manifestPkg.html
    /// </summary>
    public class PluginConfig : IPluginConfig
    {
        public string Name { get; set; }

        public string Version { get; set; }

        public string Description { get; set; }

        public string DisplayName { get; set; }

        public Uri DocumentationUrl { get; set; }

        public Uri LicenesesUrl { get; set; }

        public Dictionary<string, Version> Dependencies { get; set; }

        public string[] Keywords { get; set; }

        public string EntryName { get; set; }

        public string AuthorName { get; set; }

        public string AuthorEmail { get; set; }

        public Uri AuthorUrl { get; set; }
    }
}