using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.JellyFork.Configuration;


public class PluginConfiguration : BasePluginConfiguration
{
    public required string InputDirectory { get; set; }
    public required string OutputDirectory { get; set; }
}
