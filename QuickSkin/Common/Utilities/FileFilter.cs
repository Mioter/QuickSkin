using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace QuickSkin.Common.Utilities;

public static class FileFilter
{
    #region Public Filtering Methods

    /// <summary>
    ///     按扩展名过滤文件。
    /// </summary>
    public static IEnumerable<string> FilterByExtension(IEnumerable<string> files, params string[] extensions)
    {
        var extSet = NormalizeExtensions(extensions);

        return files.Where(f => extSet.Contains(GetFileExtension(f)));
    }

    /// <summary>
    ///     按扩展名过滤文件（从指定目录递归查找）。
    /// </summary>
    /// <param name="folderPath">要搜索的文件夹路径</param>
    /// <param name="extensions">要匹配的扩展名</param>
    /// <param name="maxDepth">最大递归深度，-1表示无限制</param>
    /// <param name="maxFiles">最大返回文件数量，-1表示无限制</param>
    public static IEnumerable<string> FilterByExtension(string folderPath, string[] extensions, int maxDepth = -1, int maxFiles = -1)
    {
        var extSet = NormalizeExtensions(extensions);

        return EnumerateFilesWithFilter(folderPath, maxDepth, maxFiles, f => extSet.Contains(GetFileExtension(f)));
    }

    /// <summary>
    ///     按文件名关键字过滤文件（支持多个关键字）。
    /// </summary>
    public static IEnumerable<string> FilterByKeyword(IEnumerable<string> files, params string[] keywords)
    {
        return files.Where(f => keywords.Any(k => GetFileName(f).Contains(k, StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>
    ///     按文件名关键字过滤文件（从指定目录递归查找）。
    /// </summary>
    /// <param name="folderPath">要搜索的文件夹路径</param>
    /// <param name="keywords">要匹配的关键字</param>
    /// <param name="maxDepth">最大递归深度，-1表示无限制</param>
    /// <param name="maxFiles">最大返回文件数量，-1表示无限制</param>
    public static IEnumerable<string> FilterByKeyword(string folderPath, string[] keywords, int maxDepth = -1, int maxFiles = -1)
    {
        return EnumerateFilesWithFilter(folderPath, maxDepth, maxFiles,
            f => keywords.Any(k => GetFileName(f).Contains(k, StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>
    ///     按正则表达式过滤文件名。
    /// </summary>
    public static IEnumerable<string> FilterByRegex(IEnumerable<string> files, string pattern)
    {
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

        return files.Where(f => regex.IsMatch(GetFileName(f)));
    }

    /// <summary>
    ///     按正则表达式过滤文件名（从指定目录递归查找）。
    /// </summary>
    /// <param name="folderPath">要搜索的文件夹路径</param>
    /// <param name="pattern">正则表达式模式</param>
    /// <param name="maxDepth">最大递归深度，-1表示无限制</param>
    /// <param name="maxFiles">最大返回文件数量，-1表示无限制</param>
    public static IEnumerable<string> FilterByRegex(string folderPath, string pattern, int maxDepth = -1, int maxFiles = -1)
    {
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

        return EnumerateFilesWithFilter(folderPath, maxDepth, maxFiles, f => regex.IsMatch(GetFileName(f)));
    }

    /// <summary>
    ///     按通配符模式过滤文件名（* 和 ?）。
    /// </summary>
    public static IEnumerable<string> FilterByWildcard(IEnumerable<string> files, string wildcard)
    {
        string regexPattern = "^" + Regex.Escape(wildcard).Replace("\\*", ".*").Replace("\\?", ".") + "$";
        var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

        return files.Where(f => regex.IsMatch(GetFileName(f)));
    }

    /// <summary>
    ///     按通配符模式过滤文件名（从指定目录递归查找）。
    /// </summary>
    /// <param name="folderPath">要搜索的文件夹路径</param>
    /// <param name="wildcard">通配符模式</param>
    /// <param name="maxDepth">最大递归深度，-1表示无限制</param>
    /// <param name="maxFiles">最大返回文件数量，-1表示无限制</param>
    public static IEnumerable<string> FilterByWildcard(string folderPath, string wildcard, int maxDepth = -1, int maxFiles = -1)
    {
        string regexPattern = "^" + Regex.Escape(wildcard).Replace("\\*", ".*").Replace("\\?", ".") + "$";
        var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

        return EnumerateFilesWithFilter(folderPath, maxDepth, maxFiles, f => regex.IsMatch(GetFileName(f)));
    }

    /// <summary>
    ///     按文件大小过滤。
    /// </summary>
    public static IEnumerable<string> FilterBySize(IEnumerable<string> files, long minSize = 0, long maxSize = long.MaxValue)
    {
        return files.WhereSafe(f =>
        {
            long size = new FileInfo(f).Length;

            return size >= minSize && size <= maxSize;
        });
    }

    /// <summary>
    ///     按文件大小过滤（从指定目录递归查找）。
    /// </summary>
    /// <param name="folderPath">要搜索的文件夹路径</param>
    /// <param name="minSize">最小文件大小</param>
    /// <param name="maxSize">最大文件大小</param>
    /// <param name="maxDepth">最大递归深度，-1表示无限制</param>
    /// <param name="maxFiles">最大返回文件数量，-1表示无限制</param>
    public static IEnumerable<string> FilterBySize(string folderPath, long minSize = 0, long maxSize = long.MaxValue, int maxDepth = -1, int maxFiles = -1)
    {
        return EnumerateFilesWithFilter(folderPath, maxDepth, maxFiles, f =>
        {
            try
            {
                long size = new FileInfo(f).Length;

                return size >= minSize && size <= maxSize;
            }
            catch
            {
                return false;
            }
        });
    }

    /// <summary>
    ///     按创建/修改时间过滤。
    /// </summary>
    public static IEnumerable<string> FilterByDate(
        IEnumerable<string> files,
        DateTime? createdAfter = null,
        DateTime? createdBefore = null,
        DateTime? modifiedAfter = null,
        DateTime? modifiedBefore = null
        )
    {
        return files.WhereSafe(f =>
        {
            var info = new FileInfo(f);
            bool createdOk = CheckDate(info.CreationTime, createdAfter, createdBefore);
            bool modifiedOk = CheckDate(info.LastWriteTime, modifiedAfter, modifiedBefore);

            return createdOk && modifiedOk;
        });
    }

    /// <summary>
    ///     按创建/修改时间过滤（从指定目录递归查找）。
    /// </summary>
    /// <param name="folderPath">要搜索的文件夹路径</param>
    /// <param name="createdAfter">创建时间之后</param>
    /// <param name="createdBefore">创建时间之前</param>
    /// <param name="modifiedAfter">修改时间之后</param>
    /// <param name="modifiedBefore">修改时间之前</param>
    /// <param name="maxDepth">最大递归深度，-1表示无限制</param>
    /// <param name="maxFiles">最大返回文件数量，-1表示无限制</param>
    public static IEnumerable<string> FilterByDate(
        string folderPath,
        DateTime? createdAfter = null,
        DateTime? createdBefore = null,
        DateTime? modifiedAfter = null,
        DateTime? modifiedBefore = null,
        int maxDepth = -1,
        int maxFiles = -1
        )
    {
        return EnumerateFilesWithFilter(folderPath, maxDepth, maxFiles, f =>
        {
            try
            {
                var info = new FileInfo(f);
                bool createdOk = CheckDate(info.CreationTime, createdAfter, createdBefore);
                bool modifiedOk = CheckDate(info.LastWriteTime, modifiedAfter, modifiedBefore);

                return createdOk && modifiedOk;
            }
            catch
            {
                return false;
            }
        });
    }

    /// <summary>
    ///     按文件属性过滤。
    /// </summary>
    public static IEnumerable<string> FilterByAttributes(IEnumerable<string> files, FileAttributes attributes, bool mustHaveAll = true)
    {
        return files.WhereSafe(f =>
        {
            var attr = new FileInfo(f).Attributes;

            return mustHaveAll ? (attr & attributes) == attributes : (attr & attributes) != 0;
        });
    }

    /// <summary>
    ///     按文件属性过滤（从指定目录递归查找）。
    /// </summary>
    /// <param name="folderPath">要搜索的文件夹路径</param>
    /// <param name="attributes">要匹配的文件属性</param>
    /// <param name="mustHaveAll">是否必须包含所有指定属性</param>
    /// <param name="maxDepth">最大递归深度，-1表示无限制</param>
    /// <param name="maxFiles">最大返回文件数量，-1表示无限制</param>
    public static IEnumerable<string> FilterByAttributes(string folderPath, FileAttributes attributes, bool mustHaveAll = true, int maxDepth = -1, int maxFiles = -1)
    {
        return EnumerateFilesWithFilter(folderPath, maxDepth, maxFiles, f =>
        {
            try
            {
                var attr = new FileInfo(f).Attributes;

                return mustHaveAll ? (attr & attributes) == attributes : (attr & attributes) != 0;
            }
            catch
            {
                return false;
            }
        });
    }

    /// <summary>
    ///     按文件内容包含指定字符串过滤。
    /// </summary>
    public static IEnumerable<string> FilterByContent(IEnumerable<string> files, string content, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        return files.WhereSafe(f => File.ReadAllText(f).Contains(content, comparison));
    }

    /// <summary>
    ///     按文件内容包含指定字符串过滤（从指定目录递归查找）。
    /// </summary>
    /// <param name="folderPath">要搜索的文件夹路径</param>
    /// <param name="content">要查找的内容</param>
    /// <param name="comparison">字符串比较方式</param>
    /// <param name="maxDepth">最大递归深度，-1表示无限制</param>
    /// <param name="maxFiles">最大返回文件数量，-1表示无限制</param>
    public static IEnumerable<string> FilterByContent(string folderPath, string content, StringComparison comparison = StringComparison.OrdinalIgnoreCase, int maxDepth = -1, int maxFiles = -1)
    {
        return EnumerateFilesWithFilter(folderPath, maxDepth, maxFiles, f =>
        {
            try
            {
                return File.ReadAllText(f).Contains(content, comparison);
            }
            catch
            {
                return false;
            }
        });
    }

    /// <summary>
    ///     去重（按路径或内容哈希）。
    /// </summary>
    public static IEnumerable<string> DistinctFiles(IEnumerable<string> files, bool byContent = false)
    {
        if (!byContent)
        {
            return files.Distinct();
        }

        var seen = new HashSet<string>();

        return files.WhereSafe(f =>
        {
            try
            {
                string hash = Convert.ToBase64String(SHA256.HashData(File.ReadAllBytes(f)));

                return seen.Add(hash);
            }
            catch
            {
                return false;
            }
        });
    }

    /// <summary>
    ///     多条件组合过滤。
    /// </summary>
    public static IEnumerable<string> Filter(IEnumerable<string> files, Func<string, bool> predicate)
    {
        return files.Where(predicate);
    }

    /// <summary>
    ///     在指定文件夹中递归查找第一个匹配文件名的文件
    /// </summary>
    /// <param name="folderPath">要搜索的文件夹路径</param>
    /// <param name="maxDepth">最大递归深度，-1表示无限制，0表示只搜索当前目录，1表示当前目录及一级子目录</param>
    /// <param name="fileNames">文件名数组（包含扩展名，如 "config.json", "data.txt"）</param>
    /// <returns>找到的文件完整路径，未找到则返回 null</returns>
    public static string? FindFirstFile(string folderPath, int maxDepth = -1, params string[] fileNames)
    {
        return FindFiles(folderPath, maxDepth, 1, fileNames).FirstOrDefault();
    }

    /// <summary>
    ///     在指定文件夹中递归查找匹配文件名的文件
    /// </summary>
    /// <param name="folderPath">要搜索的文件夹路径</param>
    /// <param name="maxDepth">最大递归深度，-1表示无限制</param>
    /// <param name="maxFiles">最大返回文件数量，-1表示无限制</param>
    /// <param name="fileNames">文件名数组（包含扩展名）</param>
    /// <returns>找到的文件完整路径集合</returns>
    public static IEnumerable<string> FindFiles(string folderPath, int maxDepth = -1, int maxFiles = -1, params string[] fileNames)
    {
        // 验证输入参数
        if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            throw new DirectoryNotFoundException($"文件夹不存在: {folderPath}");

        if (fileNames.Length == 0)
            yield break;

        // 验证递归深度参数
        if (maxDepth < -1)
            throw new ArgumentException("最大递归深度不能小于-1", nameof(maxDepth));

        if (maxFiles == 0)
            yield break;

        // 标准化文件名（转小写用于比较）
        var normalizedFileNames = new HashSet<string>(
            fileNames.Where(name => !string.IsNullOrEmpty(name)).Select(name => name.ToLower()),
            StringComparer.OrdinalIgnoreCase
        );

        if (normalizedFileNames.Count == 0)
            yield break;

        var fileCounter = new FileCounter
        {
            Count = 0,
        };

        // 如果maxDepth为-1，使用递归搜索所有目录
        if (maxDepth == -1)
        {
            var files = Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories);

            foreach (string filePath in files)
            {
                if (maxFiles > 0 && fileCounter.Count >= maxFiles)
                    yield break;

                string fileName = Path.GetFileName(filePath);

                if (normalizedFileNames.Contains(fileName))
                {
                    fileCounter.Count++;

                    yield return filePath;
                }
            }
        }
        else
        {
            // 使用自定义递归方法控制搜索深度
            foreach (string filePath in FindFilesWithDepth(folderPath, maxDepth, maxFiles, normalizedFileNames, fileCounter))
            {
                yield return filePath;
            }
        }
    }

    #endregion

    #region Private Helper Classes

    private class FileCounter
    {
        public int Count { get; set; }
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    ///     带深度和数量控制的文件查找方法
    /// </summary>
    /// <param name="folderPath">当前搜索的文件夹路径</param>
    /// <param name="remainingDepth">剩余递归深度</param>
    /// <param name="maxFiles">最大返回文件数量，-1表示无限制</param>
    /// <param name="normalizedFileNames">标准化的文件名集合</param>
    /// <param name="fileCounter">文件计数器</param>
    /// <returns>找到的文件完整路径集合</returns>
    private static IEnumerable<string> FindFilesWithDepth(string folderPath, int remainingDepth, int maxFiles, HashSet<string> normalizedFileNames, FileCounter fileCounter)
    {
        if (maxFiles > 0 && fileCounter.Count >= maxFiles)
            yield break;

        // 搜索当前目录中的文件
        IEnumerable<string> files;

        try
        {
            files = Directory.EnumerateFiles(folderPath, "*");
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"无法访问目录: {folderPath}");

            yield break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"搜索目录 {folderPath} 时发生错误: {ex.Message}");

            yield break;
        }

        foreach (string filePath in files)
        {
            if (maxFiles > 0 && fileCounter.Count >= maxFiles)
                yield break;

            string fileName = Path.GetFileName(filePath);

            if (normalizedFileNames.Contains(fileName))
            {
                fileCounter.Count++;

                yield return filePath;
            }
        }

        // 如果还有剩余深度，继续搜索子目录
        if (remainingDepth > 0)
        {
            IEnumerable<string> subDirectories;

            try
            {
                subDirectories = Directory.EnumerateDirectories(folderPath);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"无法访问目录: {folderPath}");

                yield break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"搜索目录 {folderPath} 时发生错误: {ex.Message}");

                yield break;
            }

            foreach (string subDirectory in subDirectories)
            {
                if (maxFiles > 0 && fileCounter.Count >= maxFiles)
                    yield break;

                foreach (string result in FindFilesWithDepth(subDirectory, remainingDepth - 1, maxFiles, normalizedFileNames, fileCounter))
                {
                    yield return result;
                }
            }
        }
    }

    /// <summary>
    ///     通用文件枚举方法，支持深度和数量控制
    /// </summary>
    /// <param name="folderPath">要搜索的文件夹路径</param>
    /// <param name="maxDepth">最大递归深度，-1表示无限制</param>
    /// <param name="maxFiles">最大返回文件数量，-1表示无限制</param>
    /// <param name="predicate">文件过滤条件</param>
    private static IEnumerable<string> EnumerateFilesWithFilter(string folderPath, int maxDepth, int maxFiles, Func<string, bool> predicate)
    {
        // 验证输入参数
        if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            throw new DirectoryNotFoundException($"文件夹不存在: {folderPath}");

        if (maxFiles == 0)
            yield break;

        if (maxDepth < -1)
            throw new ArgumentException("最大递归深度不能小于-1", nameof(maxDepth));

        var fileCounter = new FileCounter
        {
            Count = 0,
        };

        // 如果maxDepth为-1，使用递归搜索所有目录
        if (maxDepth == -1)
        {
            var files = Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories);

            foreach (string filePath in files)
            {
                if (maxFiles > 0 && fileCounter.Count >= maxFiles)
                    yield break;

                if (!predicate(filePath))
                    continue;

                fileCounter.Count++;

                yield return filePath;
            }
        }
        else
        {
            // 使用自定义递归方法控制搜索深度
            foreach (string filePath in EnumerateFilesWithDepth(folderPath, maxDepth, maxFiles, predicate, fileCounter))
            {
                yield return filePath;
            }
        }
    }

    /// <summary>
    ///     带深度和数量控制的通用文件枚举方法
    /// </summary>
    private static IEnumerable<string> EnumerateFilesWithDepth(string folderPath, int remainingDepth, int maxFiles, Func<string, bool> predicate, FileCounter fileCounter)
    {
        if (maxFiles > 0 && fileCounter.Count >= maxFiles)
            yield break;

        // 搜索当前目录中的文件
        IEnumerable<string> files;

        try
        {
            files = Directory.EnumerateFiles(folderPath, "*");
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"无法访问目录: {folderPath}");

            yield break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"搜索目录 {folderPath} 时发生错误: {ex.Message}");

            yield break;
        }

        foreach (string filePath in files)
        {
            if (maxFiles > 0 && fileCounter.Count >= maxFiles)
                yield break;

            if (!predicate(filePath)) 
                continue;

            fileCounter.Count++;

            yield return filePath;
        }

        // 如果还有剩余深度，继续搜索子目录
        if (remainingDepth > 0)
        {
            IEnumerable<string> subDirectories;

            try
            {
                subDirectories = Directory.EnumerateDirectories(folderPath);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"无法访问目录: {folderPath}");

                yield break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"搜索目录 {folderPath} 时发生错误: {ex.Message}");

                yield break;
            }

            foreach (string subDirectory in subDirectories)
            {
                if (maxFiles > 0 && fileCounter.Count >= maxFiles)
                    yield break;

                foreach (string result in EnumerateFilesWithDepth(subDirectory, remainingDepth - 1, maxFiles, predicate, fileCounter))
                {
                    yield return result;
                }
            }
        }
    }

    private static HashSet<string> NormalizeExtensions(IEnumerable<string> extensions)
    {
        return new HashSet<string>(
            extensions
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Select(NormalizeExtension),
            StringComparer.OrdinalIgnoreCase
        );
    }

    private static string NormalizeExtension(string ext)
    {
        if (string.IsNullOrEmpty(ext)) return string.Empty;

        return ext.StartsWith('.') ? ext.ToLower() : "." + ext.ToLower();
    }

    private static string GetFileExtension(string filePath)
    {
        return Path.GetExtension(filePath).ToLower();
    }

    private static string GetFileName(string filePath)
    {
        return Path.GetFileName(filePath);
    }

    private static bool CheckDate(DateTime fileTime, DateTime? after, DateTime? before)
    {
        return (!after.HasValue || fileTime >= after) &&
            (!before.HasValue || fileTime <= before);
    }

    private static IEnumerable<string> WhereSafe(this IEnumerable<string> files, Func<string, bool> predicate)
    {
        foreach (string file in files)
        {
            bool shouldInclude = false;

            try
            {
                shouldInclude = predicate(file);
            }
            catch
            {
                // 忽略异常，shouldInclude 保持 false
            }

            if (shouldInclude)
                yield return file;
        }
    }

    #endregion
}
