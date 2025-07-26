using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using QuickSkin.Common.Services;

namespace QuickSkin.Common.Utilities;

public static class FileInteraction
{
    /// <summary>
    ///     在系统默认文件管理器中打开指定文件或目录
    /// </summary>
    /// <param name="path">文件或目录路径</param>
    public static void OpenInFileManager(string path)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start(
                new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = File.Exists(path) ? $"/select,\"{path}\"" : $"\"{path}\"",
                    UseShellExecute = true,
                }
            );
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start(
                new ProcessStartInfo
                {
                    FileName = "open",
                    Arguments = File.Exists(path) ? $"-R \"{path}\"" : $"\"{path}\"",
                    UseShellExecute = true,
                }
            );
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start(
                new ProcessStartInfo
                {
                    FileName = "xdg-open",
                    Arguments = $"\"{(Directory.Exists(path) ? path : Path.GetDirectoryName(path))}\"",
                    UseShellExecute = true,
                }
            );
        }
        else
        {
            NotificationService.Error("当前操作系统不支持此操作");
        }
    }
}
