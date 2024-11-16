using System;
using System.Collections.Generic;
using System.Globalization;
using System.Collections.Generic;

using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

using Jellyfin.Plugin.JellyFork.Configuration;
using Microsoft.Extensions.Logging;


namespace Jellyfin.Plugin.JellyFork;

public class JellyForkPlugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    private readonly ILogger<JellyForkPlugin> _logger;

    public JellyForkPlugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILogger<JellyForkPlugin> logger)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
        _logger = logger;
    }
    public override string Name => "JellyFork";
    public override string Description => "Move and rename new downloads to media directory";
    public override Guid Id => Guid.Parse("6aabd9ed-969f-4225-b316-6b3b33a2fddb");
    public static JellyForkPlugin? Instance { get; private set; }


    public IEnumerable<PluginPageInfo> GetPages()
    {
        return
        [
            new PluginPageInfo
            {
                Name = Name,
                EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Configuration.configPage.html", GetType().Namespace),
            }
        ];
    }
}

