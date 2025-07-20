using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Serilog;

namespace QuickSkin.Common.Services;

/// <summary>
///     支持 AOT 且完全自定义结构的 JSON 配置服务，需借助源生成器
/// </summary>
public class JsonConfigService<T>(
    string fileName,
    JsonSerializerContext jsonSerializerContext,
    ILogger? logger = null,
    string? fileExtension = null)
    where T : new()
{
    private readonly string _fullPath = Path.Combine(StaticConfig.ConfigPath, $"{fileName}{fileExtension ?? ".QwQ.json"}");
    private readonly ILogger _logger = logger ?? Log.Logger;

    public void Save(T data)
    {
        try
        {
            EnsureDirectory();
            string json = JsonSerializer.Serialize(data, typeof(T), jsonSerializerContext);
            File.WriteAllText(_fullPath, json);
            _logger.Information("配置已保存到 {Path}", _fullPath);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "保存配置到 {Path} 时发生异常", _fullPath);

            throw;
        }
    }

    public async Task SaveAsync(T data)
    {
        try
        {
            EnsureDirectory();
            string json = JsonSerializer.Serialize(data, typeof(T), jsonSerializerContext);
            await File.WriteAllTextAsync(_fullPath, json);
            _logger.Information("配置已异步保存到 {Path}", _fullPath);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "异步保存配置到 {Path} 时发生异常", _fullPath);

            throw;
        }
    }

    public T Load()
    {
        try
        {
            if (!File.Exists(_fullPath))
            {
                _logger.Warning("配置文件 {Path} 不存在，返回默认值", _fullPath);

                return new T();
            }

            string json = File.ReadAllText(_fullPath);

            if (jsonSerializerContext.GetTypeInfo(typeof(T)) is not JsonTypeInfo<T> jsonTypeInfo)
            {
                _logger.Warning("配置 {Path} 未能正常加载: 类型不匹配", _fullPath);

                return new T();
            }

            var result = JsonSerializer.Deserialize(json, jsonTypeInfo);

            if (result is null)
            {
                _logger.Warning("配置 {Path} 未能正常加载: 结果为空", _fullPath);
            }
            else
            {
                _logger.Information("配置已从 {Path} 加载", _fullPath);
            }

            return result ?? new T();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "加载配置文件 {Path} 时发生异常", _fullPath);

            throw;
        }
    }

    public async Task<T> LoadAsync()
    {
        try
        {
            if (!File.Exists(_fullPath))
            {
                _logger.Warning("配置文件 {Path} 不存在，返回默认值", _fullPath);

                return new T();
            }

            string json = await File.ReadAllTextAsync(_fullPath);

            if (jsonSerializerContext.GetTypeInfo(typeof(T)) is not JsonTypeInfo<T> jsonTypeInfo)
            {
                _logger.Warning("配置 {Path} 未能正常加载: 类型不匹配", _fullPath);

                return new T();
            }

            var result = JsonSerializer.Deserialize(json, jsonTypeInfo);

            if (result is null)
            {
                _logger.Warning("配置 {Path} 未能正常加载: 结果为空", _fullPath);
            }
            else
            {
                _logger.Information("配置已异步从 {Path} 加载", _fullPath);
            }

            return result ?? new T();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "异步加载配置文件 {Path} 时发生异常", _fullPath);

            throw;
        }
    }

    private void EnsureDirectory()
    {
        string? dir = Path.GetDirectoryName(_fullPath);

        if (string.IsNullOrEmpty(dir) || Directory.Exists(dir))
            return;

        Directory.CreateDirectory(dir);
        _logger.Information("已创建配置目录 {Dir}", dir);
    }
}
