namespace QuickSkin.Models.Configs;

public class ConfigModel
{
    public ToolsConfig ToolsConfig { get; set; } = new();

    public UiConfig UiConfig { get; set; } = new();

    public SystemConfig SystemConfig { get; set; } = new();
}
