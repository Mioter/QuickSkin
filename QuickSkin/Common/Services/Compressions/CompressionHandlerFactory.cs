using System;
using System.Collections.Generic;
using System.IO;

namespace QuickSkin.Common.Services.Compressions;

public static class CompressionHandlerFactory
{
    private static readonly Dictionary<string, Func<ICompressionHandler>> _handlerCreators = new()
    {
        { ".zip", () => new ZipCompressionHandler() },
        { ".rar", () => new RarCompressionHandler() },
        { ".7z", () => new SevenZipCompressionHandler() },
        { ".tar", () => new TarCompressionHandler() },
        { ".tar.gz", () => new TarCompressionHandler() },
        { ".tgz", () => new TarCompressionHandler() },
        { ".tar.bz2", () => new TarCompressionHandler() }
    };

    private static readonly HashSet<string> _supportedExtensions = new(_handlerCreators.Keys, StringComparer.OrdinalIgnoreCase);

    // 根据扩展名获取处理器
    public static ICompressionHandler? GetHandler(string extension)
    {
        if (string.IsNullOrEmpty(extension))
            return null;

        // 确保扩展名以点开头
        if (!extension.StartsWith('.'))
            extension = "." + extension;

        // 直接查找，避免反射
        return _handlerCreators.TryGetValue(extension.ToLower(), out var creator) 
            ? creator() 
            : null;
    }

    // 根据文件路径获取处理器
    public static ICompressionHandler? GetHandlerByPath(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return null;

        string extension = Path.GetExtension(filePath);
        return GetHandler(extension);
    }

    // 获取所有支持的扩展名
    public static IEnumerable<string> GetAllSupportedExtensions()
    {
        return _supportedExtensions;
    }

    // 检查扩展名是否支持
    public static bool IsExtensionSupported(string extension)
    {
        if (string.IsNullOrEmpty(extension))
            return false;

        if (!extension.StartsWith('.'))
            extension = "." + extension;

        return _supportedExtensions.Contains(extension.ToLower());
    }

    // 获取所有处理器信息（用于UI显示）
    public static IEnumerable<(string DisplayName, string[] Extensions)> GetAllHandlerInfo()
    {
        yield return (new ZipCompressionHandler().DisplayName, new ZipCompressionHandler().SupportedExtensions);
        yield return (new RarCompressionHandler().DisplayName, new RarCompressionHandler().SupportedExtensions);
        yield return (new SevenZipCompressionHandler().DisplayName, new SevenZipCompressionHandler().SupportedExtensions);
        yield return (new TarCompressionHandler().DisplayName, new TarCompressionHandler().SupportedExtensions);
    }
}