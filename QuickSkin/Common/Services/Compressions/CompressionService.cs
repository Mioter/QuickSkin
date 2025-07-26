using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace QuickSkin.Common.Services.Compressions;

public class CompressionService
{
    
    
    // 压缩文件
    public static async Task<bool> CompressFilesAsync(string[] filePaths, string outputPath, IProgress<int>? progress = null)
    {
        Directory.CreateDirectory(outputPath);
        var handler = CompressionHandlerFactory.GetHandlerByPath(outputPath);
        if (handler == null)
            throw new NotSupportedException($"不支持的压缩格式: {Path.GetExtension(outputPath)}");

        return await handler.CompressAsync(filePaths, outputPath, progress);
    }

    // 压缩文件夹
    public static async Task<bool> CompressFolderAsync(string folderPath, string outputPath, IProgress<int>? progress = null)
    {
        Directory.CreateDirectory(outputPath);
        var handler = CompressionHandlerFactory.GetHandlerByPath(outputPath);
        if (handler == null)
            throw new NotSupportedException($"不支持的压缩格式: {Path.GetExtension(outputPath)}");

        return await handler.CompressFolderAsync(folderPath, outputPath, progress);
    }

    // 解压文件
    public static async Task<bool> ExtractAsync(string archivePath, string extractPath, IProgress<int>? progress = null)
    {
        Directory.CreateDirectory(extractPath);
        var handler = CompressionHandlerFactory.GetHandlerByPath(archivePath);
        if (handler == null)
            throw new NotSupportedException($"不支持的压缩格式: {Path.GetExtension(archivePath)}");

        return await handler.ExtractAsync(archivePath, extractPath, progress);
    }

    // 解压文件
    public static Task<bool> ExtractAsync(string archivePath, string[] extractPaths, IProgress<int>? progress = null)
    {
        string extractPath = Path.Combine(extractPaths);
        return ExtractAsync(archivePath, extractPath, progress);
    }

    // 获取支持的格式信息
    public static IEnumerable<(string DisplayName, string[] Extensions)> GetSupportedFormats()
    {
        return CompressionHandlerFactory.GetAllHandlerInfo();
    }
}