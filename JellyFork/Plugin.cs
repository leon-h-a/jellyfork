using System;
using System.Collections.Generic;
using System.Globalization;
using Jellyfin.Plugin.JellyFork.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;

namespace Jellyfin.Plugin.JellyFork;

public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    private readonly ILogger<Plugin> _logger;

    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILogger<Plugin> logger)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
        _logger = logger;
    }
    public override string Name => "JellyFork";
    public override string Description => "Move and rename new downloads to media directory";
    public override Guid Id => Guid.Parse("6aabd9ed-969f-4225-b316-6b3b33a2fddb");
    public static Plugin? Instance { get; private set; }

    public IEnumerable<PluginPageInfo> GetPages()
    {
        return
        [
            new PluginPageInfo
            {
                Name = Name,
                EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Configuration.configPage.html", GetType().Namespace)
            }
        ];
    }
}

