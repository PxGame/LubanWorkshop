using System;
using System.Collections.Generic;

namespace Luban.Core.Services.Plugins
{
    public interface IPluginConfig
    {
        string Name { get; }
        string Version { get; }
        string Description { get; }
        string DisplayName { get; }
        Uri DocumentationUrl { get; }
        Uri LicenesesUrl { get; }
        Dictionary<string, Version> Dependencies { get; }
        string[] Keywords { get; }
        string EntryName { get; }
        string AuthorName { get; }
        string AuthorEmail { get; }
        Uri AuthorUrl { get; }
    }
}