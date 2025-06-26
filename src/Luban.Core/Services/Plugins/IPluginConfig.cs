using System;
using System.Collections.Generic;

namespace Luban.Core.Services.Plugins
{
    [Flags]
    public enum PluginPlatform
    {
        None = 0x0000_0000,

        Windows = 0x0000_0001,
        Linux = 0x0000_0010,
        OSX = 0x0000_0100,
        Android = 0x0000_1000,
        IOS = 0x0001_0000,
        Web = 0x0010_0000,

        Desktop = Windows | Linux | OSX,
        Mobile = Android | IOS,
        All = Windows | Linux | OSX | Android | IOS | Web
    }

    public interface IPluginConfig
    {
        string Name { get; }
        string Version { get; }
        string Description { get; }
        string DisplayName { get; }
        PluginPlatform Platforms { get; }
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