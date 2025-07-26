using System;
using System.Threading.Tasks;

namespace QuickSkin.Common.Services.Compressions; // 压缩处理器接口

public interface ICompressionHandler
{
    Task<bool> CompressAsync(string[] filePaths, string outputPath, IProgress<int>? progress = null);
    Task<bool> CompressFolderAsync(string folderPath, string outputPath, IProgress<int>? progress = null);
    Task<bool> ExtractAsync(string archivePath, string extractPath, IProgress<int>? progress = null);
    string[] SupportedExtensions { get; }
    string DisplayName { get; }
}