using System.Text.Json.Serialization;
using QuickSkin.Common.Services;
using QuickSkin.Models;

namespace QuickSkin.Common.Manager;

public static class ConfigManager
{
    private static readonly JsonConfigService<ConfigModel> _service = new(
        nameof(ConfigManager).ToLower(),
        ConfigModelJsonContext.Default
    );

    public static ConfigModel ConfigModel { get; set; }

    static ConfigManager()
    {
        // 静态构造时自动加载配置（同步）
        ConfigModel = _service.Load();
    }

    public static void Save()
    {
        _service.Save(ConfigModel);
    }
}

[JsonSerializable(typeof(ConfigModel))]
public partial class ConfigModelJsonContext : JsonSerializerContext;
