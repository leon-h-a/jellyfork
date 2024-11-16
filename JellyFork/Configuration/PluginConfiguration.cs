using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.JellyFork.Configuration;

public class PluginConfiguration : BasePluginConfiguration
{
    public PluginConfiguration()
    {
        InputDir = "/input/directory";
        OutputDir = "/input/directory";
    }

    public string InputDir { get; set; }
    public string OutputDir { get; set; }
}
