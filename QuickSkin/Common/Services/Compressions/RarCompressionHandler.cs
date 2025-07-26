using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;

namespace QuickSkin.Common.Services.Compressions;

public class RarCompressionHandler : ICompressionHandler
{
    public string[] SupportedExtensions => [".rar"];
    public string DisplayName => "RAR压缩";

    public async Task<bool> CompressAsync(string[] filePaths, string outputPath, IProgress<int>? progress = null)
    {
        // 注意：SharpCompress主要用于解压RAR，目前并不能创建RAR
        return await Task.Run(() =>
        {
            try
            {
                // RAR格式不支持创建，直接抛出未实现异常
                throw new NotImplementedException("当前不支持创建RAR压缩文件");
            }
            catch
            {
                return false;
            }
        });
    }

    public async Task<bool> CompressFolderAsync(string folderPath, string outputPath, IProgress<int>? progress = null)
    {
        return await Task.Run(() =>
        {
            try
            {
                // RAR格式不支持创建，直接抛出未实现异常
                throw new NotImplementedException("当前不支持创建RAR压缩文件");
            }
            catch
            {
                return false;
            }
        });
    }

    public async Task<bool> ExtractAsync(string archivePath, string extractPath, IProgress<int>? progress = null)
    {
        return await Task.Run(() =>
        {
            try
            {
                using var archive = RarArchive.Open(archivePath);
                var entries = new List<RarArchiveEntry>(archive.Entries);
                int totalEntries = entries.Count;
                
                for (int i = 0; i < totalEntries; i++)
                {
                    var entry = entries[i];
                    if (!entry.IsDirectory)
                    {
                        entry.WriteToDirectory(extractPath, 
                            new ExtractionOptions { ExtractFullPath = true, Overwrite = true });
                    }
                    progress?.Report((i + 1) * 100 / totalEntries);
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        });
    }
}