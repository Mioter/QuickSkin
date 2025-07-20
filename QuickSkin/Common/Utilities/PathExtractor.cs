using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Platform.Storage;
using Serilog;

namespace QuickSkin.Common.Utilities;

public static class PathExtractor
{
    /// <summary>
    ///     将存储项目转换为路径字符串列表
    /// </summary>
    /// <param name="items">存储项目集合</param>
    /// <returns>路径字符串列表</returns>
    public static IEnumerable<string> StorageItemsToStrings(IEnumerable<IStorageItem> items)
    {
        return items.Where(item => item.Path.IsAbsoluteUri).Select(item => item.Path.LocalPath);
    }

    /// <summary>
    ///     从路径项获取所有文件路径（包括子目录）
    /// </summary>
    /// <param name="items">路径列表</param>
    /// <returns>所有文件路径</returns>
    public static IEnumerable<string> GetAllFilePaths(IEnumerable<IStorageItem> items)
    {
        return GetAllFilePaths(StorageItemsToStrings(items));
    }

    /// <summary>
    ///     从路径文本列表获取所有文件路径（包括子目录）
    /// </summary>
    /// <param name="paths">路径列表</param>
    /// <returns>所有文件路径</returns>
    public static IEnumerable<string> GetAllFilePaths(IEnumerable<string> paths)
    {
        foreach (string path in paths.Where(p => !string.IsNullOrWhiteSpace(p)))
        {
            if (Directory.Exists(path))
            {
                string[] files = [];

                try
                {
                    files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                }
                catch (Exception ex)
                {
                    Log.Error("无法访问目录 {path}: {Message}", path, ex.Message);
                }

                foreach (string file in files)
                {
                    yield return file;
                }
            }
            else if (File.Exists(path))
            {
                yield return path;
            }
            else
            {
                Log.Error("路径不存在: {path}", path);
            }
        }
    }
}
