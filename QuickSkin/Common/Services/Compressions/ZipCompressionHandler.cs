using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;

namespace QuickSkin.Common.Services.Compressions;

public class ZipCompressionHandler : ICompressionHandler
{
    public string[] SupportedExtensions => [".zip"];
    public string DisplayName => "ZIP压缩";

    public async Task<bool> CompressAsync(string[] filePaths, string outputPath, IProgress<int>? progress = null)
    {
        return await Task.Run(() =>
        {
            try
            {
                using var archive = ZipArchive.Create();
                int totalFiles = filePaths.Length;
                
                for (int i = 0; i < totalFiles; i++)
                {
                    string filePath = filePaths[i];
                    if (File.Exists(filePath))
                    {
                        archive.AddEntry(Path.GetFileName(filePath), filePath);
                    }
                    
                    progress?.Report((i + 1) * 100 / totalFiles);
                }
                
                archive.SaveTo(outputPath, CompressionType.Deflate);
                return true;
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
                using var archive = ZipArchive.Create();
                
                string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
                int totalFiles = files.Length;
                
                for (int i = 0; i < totalFiles; i++)
                {
                    string filePath = files[i];
                    string relativePath = filePath[(folderPath.Length + 1)..];
                    archive.AddEntry(relativePath, filePath);
                    
                    progress?.Report((i + 1) * 100 / totalFiles);
                }
                
                archive.SaveTo(outputPath, CompressionType.Deflate);
                return true;
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
                using var archive = ZipArchive.Open(archivePath);
                var entries = new List<ZipArchiveEntry>(archive.Entries);
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