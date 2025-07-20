using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace QuickSkin.Common.Utilities;

public static class FileFilter
{
    /// <summary>
    ///     按扩展名过滤文件。
    /// </summary>
    /// <param name="files">文件路径集合</param>
    /// <param name="extensions">扩展名集合（如 .txt, .jpg），不区分大小写，自动补全“.”前缀</param>
    /// <returns>过滤后的文件路径集合</returns>
    /// <remarks>忽略空扩展名，返回包含指定扩展名的所有文件</remarks>
    public static IEnumerable<string> FilterByExtension(IEnumerable<string> files, params string[] extensions)
    {
        var extSet = new HashSet<string>(extensions.Where(e => !string.IsNullOrWhiteSpace(e)).Select(e => e.StartsWith('.') ? e.ToLower() : "." + e.ToLower()));

        return files.Where(f => extSet.Contains(Path.GetExtension(f).ToLower()));
    }

    /// <summary>
    ///     按文件名关键字过滤文件（支持多个关键字）。
    /// </summary>
    /// <param name="files">文件路径集合</param>
    /// <param name="keywords">关键字数组，文件名包含任一关键字即匹配</param>
    /// <returns>过滤后的文件路径集合</returns>
    public static IEnumerable<string> FilterByKeyword(IEnumerable<string> files, params string[] keywords)
    {
        return files.Where(f => keywords.Any(k => Path.GetFileName(f).Contains(k, StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>
    ///     按文件名正则表达式过滤。
    /// </summary>
    /// <param name="files">文件路径集合</param>
    /// <param name="pattern">正则表达式，匹配文件名</param>
    /// <returns>过滤后的文件路径集合</returns>
    /// <exception cref="ArgumentException">正则表达式无效时抛出</exception>
    public static IEnumerable<string> FilterByRegex(IEnumerable<string> files, string pattern)
    {
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

        return files.Where(f => regex.IsMatch(Path.GetFileName(f)));
    }

    /// <summary>
    ///     按文件名通配符（如*.txt、data_*.csv）过滤。
    /// </summary>
    /// <param name="files">文件路径集合</param>
    /// <param name="wildcard">通配符字符串（*代表任意字符，?代表单个字符）</param>
    /// <returns>过滤后的文件路径集合</returns>
    public static IEnumerable<string> FilterByWildcard(IEnumerable<string> files, string wildcard)
    {
        string regexPattern = "^" + Regex.Escape(wildcard).Replace("\\*", ".*").Replace("\\?", ".") + "$";
        var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

        return files.Where(f => regex.IsMatch(Path.GetFileName(f)));
    }

    /// <summary>
    ///     按文件大小过滤文件。
    /// </summary>
    /// <param name="files">文件路径集合</param>
    /// <param name="minSize">最小文件大小（字节），默认0</param>
    /// <param name="maxSize">最大文件大小（字节），默认long.MaxValue</param>
    /// <returns>过滤后的文件路径集合</returns>
    /// <remarks>无法访问的文件将被自动跳过</remarks>
    public static IEnumerable<string> FilterBySize(IEnumerable<string> files, long minSize = 0, long maxSize = long.MaxValue)
    {
        return files.Where(f =>
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
    ///     按创建/修改时间过滤文件。
    /// </summary>
    /// <param name="files">文件路径集合</param>
    /// <param name="createdAfter">创建时间晚于（含）</param>
    /// <param name="createdBefore">创建时间早于（含）</param>
    /// <param name="modifiedAfter">修改时间晚于（含）</param>
    /// <param name="modifiedBefore">修改时间早于（含）</param>
    /// <returns>过滤后的文件路径集合</returns>
    /// <remarks>任意参数为null则不限制该条件</remarks>
    public static IEnumerable<string> FilterByDate(IEnumerable<string> files, DateTime? createdAfter = null, DateTime? createdBefore = null, DateTime? modifiedAfter = null, DateTime? modifiedBefore = null)
    {
        return files.Where(f =>
        {
            try
            {
                var info = new FileInfo(f);
                bool createdOk = (!createdAfter.HasValue || info.CreationTime >= createdAfter) && (!createdBefore.HasValue || info.CreationTime <= createdBefore);
                bool modifiedOk = (!modifiedAfter.HasValue || info.LastWriteTime >= modifiedAfter) && (!modifiedBefore.HasValue || info.LastWriteTime <= modifiedBefore);

                return createdOk && modifiedOk;
            }
            catch { return false; }
        });
    }

    /// <summary>
    ///     按文件属性过滤（如只读、隐藏）。
    /// </summary>
    /// <param name="files">文件路径集合</param>
    /// <param name="attributes">要匹配的文件属性（可组合）</param>
    /// <param name="mustHaveAll">是否必须全部包含这些属性，false则只要包含任意一个</param>
    /// <returns>过滤后的文件路径集合</returns>
    public static IEnumerable<string> FilterByAttributes(IEnumerable<string> files, FileAttributes attributes, bool mustHaveAll = true)
    {
        return files.Where(f =>
        {
            try
            {
                var attr = new FileInfo(f).Attributes;

                return mustHaveAll ? (attr & attributes) == attributes : (attr & attributes) != 0;
            }
            catch { return false; }
        });
    }

    /// <summary>
    ///     按文件内容包含指定字符串过滤（适合小文件）。
    /// </summary>
    /// <param name="files">文件路径集合</param>
    /// <param name="content">要查找的内容字符串</param>
    /// <param name="comparison">字符串比较方式，默认忽略大小写</param>
    /// <returns>过滤后的文件路径集合</returns>
    /// <remarks>大文件慎用，无法访问的文件自动跳过</remarks>
    public static IEnumerable<string> FilterByContent(IEnumerable<string> files, string content, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        return files.Where(f =>
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
    ///     递归获取目录下所有文件。
    /// </summary>
    /// <param name="directory">根目录路径</param>
    /// <param name="searchPattern">搜索模式，默认“*”</param>
    /// <param name="option">是否递归，默认AllDirectories</param>
    /// <returns>所有匹配的文件路径集合</returns>
    /// <remarks>目录不存在或无权限时返回空集合</remarks>
    public static IEnumerable<string> GetFilesRecursive(string directory, string searchPattern = "*", SearchOption option = SearchOption.AllDirectories)
    {
        try
        {
            return Directory.EnumerateFiles(directory, searchPattern, option);
        }
        catch { return []; }
    }

    /// <summary>
    ///     去重（按路径或内容哈希）。
    /// </summary>
    /// <param name="files">文件路径集合</param>
    /// <param name="byContent">true则按内容哈希去重，false按路径去重</param>
    /// <returns>去重后的文件路径集合</returns>
    /// <remarks>按内容去重时，无法读取的文件自动跳过</remarks>
    public static IEnumerable<string> DistinctFiles(IEnumerable<string> files, bool byContent = false)
    {
        if (!byContent)
        {
            var set = new HashSet<string>();

            foreach (string f in files)
            {
                if (set.Add(f))
                    yield return f;
            }

            yield break;
        }

        var seen = new HashSet<string>();

        foreach (string f in files)
        {
            string? hash = null;

            try
            {
                hash = Convert.ToBase64String(SHA256.HashData(File.ReadAllBytes(f)));
            }
            catch
            {
                // ignored
            }

            if (hash != null && seen.Add(hash))
                yield return f;
        }
    }

    /// <summary>
    ///     多条件组合过滤。
    /// </summary>
    /// <param name="files">文件路径集合</param>
    /// <param name="predicate">自定义过滤条件委托</param>
    /// <returns>过滤后的文件路径集合</returns>
    public static IEnumerable<string> Filter(IEnumerable<string> files, Func<string, bool> predicate)
    {
        return files.Where(predicate);
    }
}
