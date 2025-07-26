using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace QuickSkin.Common.Utilities;

public static class PathSelector
{
    /// <summary>
    ///     打开文件夹选择器并返回选中目录（单选）
    /// </summary>
    public static async Task<string?> OpenFolderAsync(TopLevel topLevel)
    {
        var items = await topLevel.StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions
            {
                Title = "选择文件夹",
                AllowMultiple = false,
            }
        );

        return items.Count > 0 ? items[0].Path.LocalPath : null;
    }

    /// <summary>
    ///     打开文件选择器（单选），返回文件路径
    /// </summary>
    public static async Task<string?> OpenFileAsync(TopLevel topLevel, string? title = null, IEnumerable<FilePickerFileType>? fileTypes = null)
    {
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title ?? "选择文件",
            AllowMultiple = false,
            FileTypeFilter = fileTypes?.ToList(),
        });

        return files.Count > 0 ? files[0].Path.LocalPath : null;
    }

    /// <summary>
    ///     打开文件选择器（多选），返回文件路径列表
    /// </summary>
    public static async Task<List<string>> OpenFilesAsync(TopLevel topLevel, string? title = null, IEnumerable<FilePickerFileType>? fileTypes = null)
    {
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title ?? "选择文件",
            AllowMultiple = true,
            FileTypeFilter = fileTypes?.ToList(),
        });

        return files.Select(f => f.Path.LocalPath).ToList();
    }

    /// <summary>
    ///     打开保存文件对话框，返回保存路径
    /// </summary>
    public static async Task<string?> SaveFileAsync(TopLevel topLevel, string? title = null, IEnumerable<FilePickerFileType>? fileTypes = null, string? suggestedFileName = null)
    {
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = title ?? "保存文件",
            FileTypeChoices = fileTypes?.ToList(),
            SuggestedFileName = suggestedFileName,
        });

        return file?.Path.LocalPath;
    }
}
