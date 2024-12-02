using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.Template.Configuration;

public enum SomeOptions
{
    OneOption,
    AnotherOption
}

public class PluginConfiguration : BasePluginConfiguration
{
    public PluginConfiguration()
    {
        Options = SomeOptions.AnotherOption;
        TrueFalseSetting = true;
        AnInteger = 2;
        AString = "string";
    }
    public bool TrueFalseSetting { get; set; }
    public int AnInteger { get; set; }
    public string AString { get; set; }
    public SomeOptions Options { get; set; }
}
