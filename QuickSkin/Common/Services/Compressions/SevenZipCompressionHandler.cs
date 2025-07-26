using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;

namespace QuickSkin.Common.Services.Compressions;

public class SevenZipCompressionHandler : ICompressionHandler
{
    public string[] SupportedExtensions => [".7z"];
    public string DisplayName => "7Z压缩";

    public async Task<bool> CompressAsync(string[] filePaths, string outputPath, IProgress<int>? progress = null)
    {
        return await Task.Run(() =>
        {
            try
            {
                throw new NotImplementedException("当前不支持创建7z压缩文件");
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
                throw new NotImplementedException("当前不支持创建7z压缩文件");
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
                using var archive = SevenZipArchive.Open(archivePath);
                var entries = new List<SevenZipArchiveEntry>(archive.Entries);
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