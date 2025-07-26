using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace QuickSkin.Common.Manager;

/// <summary>
///     资源字典动态管理器
/// </summary>
public static class ResDicManager
{
    // 存储已加载的资源字典及其对应的URI
    private static readonly Dictionary<string, ResourceDictionary> _loadedResourceDictionaries = new();

    /// <summary>
    ///     加载资源字典文件
    /// </summary>
    /// <param name="uri">资源字典文件的URI</param>
    /// <param name="key">资源字典的唯一标识符</param>
    /// <returns>如果成功加载资源字典，则返回 true；否则返回 false。</returns>
    public static bool LoadResourceDictionary(Uri uri, string key)
    {
        try
        {
            // 检查是否已加载相同键的资源字典
            if (_loadedResourceDictionaries.ContainsKey(key))
            {
                return false; // 已存在相同键的资源字典
            }

            // 加载资源字典
            var resourceDictionary = (ResourceDictionary)AvaloniaXamlLoader.Load(uri);

            // 将资源字典添加到应用程序的合并字典中
            if (Application.Current == null) return false;

            // 添加到合并字典
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);

            // 保存引用以便后续卸载
            _loadedResourceDictionaries[key] = resourceDictionary;

            return true;
        }
        catch (Exception ex)
        {
            // 记录异常信息
            Debug.WriteLine($"加载资源字典失败: {ex.Message}");

            return false;
        }
    }

    /// <summary>
    ///     卸载资源字典
    /// </summary>
    /// <param name="key">资源字典的唯一标识符</param>
    /// <returns>如果成功卸载资源字典，则返回 true；否则返回 false。</returns>
    public static bool UnloadResourceDictionary(string key)
    {
        try
        {
            // 检查资源字典是否已加载
            if (!_loadedResourceDictionaries.TryGetValue(key, out var resourceDictionary))
            {
                return false; // 未找到指定键的资源字典
            }

            // 从应用程序的合并字典中移除
            if (Application.Current == null) return false;

            Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);

            // 从已加载字典集合中移除
            _loadedResourceDictionaries.Remove(key);

            return true;
        }
        catch (Exception ex)
        {
            // 记录异常信息
            Debug.WriteLine($"卸载资源字典失败: {ex.Message}");

            return false;
        }
    }

    /// <summary>
    ///     获取已加载的资源字典列表
    /// </summary>
    /// <returns>已加载的资源字典键列表</returns>
    public static IEnumerable<string> GetLoadedResourceDictionaryKeys()
    {
        return _loadedResourceDictionaries.Keys;
    }

    /// <summary>
    ///     检查资源字典是否已加载
    /// </summary>
    /// <param name="key">资源字典的唯一标识符</param>
    /// <returns>如果资源字典已加载，则返回 true；否则返回 false。</returns>
    public static bool IsResourceDictionaryLoaded(string key)
    {
        return _loadedResourceDictionaries.ContainsKey(key);
    }

    /// <summary>
    ///     从文件加载资源字典
    /// </summary>
    /// <param name="filePath">资源字典文件的路径</param>
    /// <param name="key">资源字典的唯一标识符</param>
    /// <returns>如果成功加载资源字典，则返回 true；否则返回 false。</returns>
    public static bool LoadResourceDictionaryFromFile(string filePath, string key)
    {
        if (!File.Exists(filePath))
        {
            return false;
        }

        try
        {
            // 创建URI
            var uri = new Uri(filePath, UriKind.Absolute);

            return LoadResourceDictionary(uri, key);
        }
        catch (Exception ex)
        {
            // 记录异常信息
            Debug.WriteLine($"从文件加载资源字典失败: {ex.Message}");

            return false;
        }
    }

    /// <summary>
    ///     从嵌入资源加载资源字典
    /// </summary>
    /// <param name="resourceUri">资源URI，例如 "avares://AssemblyName/Assets/Theme.axaml"</param>
    /// <param name="key">资源字典的唯一标识符</param>
    /// <returns>如果成功加载资源字典，则返回 true；否则返回 false。</returns>
    public static bool LoadResourceDictionaryFromResource(string resourceUri, string key)
    {
        try
        {
            // 创建URI
            var uri = new Uri(resourceUri);

            return LoadResourceDictionary(uri, key);
        }
        catch (Exception ex)
        {
            // 记录异常信息
            Debug.WriteLine($"从资源加载资源字典失败: {ex.Message}");

            return false;
        }
    }
}
