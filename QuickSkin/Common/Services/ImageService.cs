using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using QuickSkin.Common.Manager;
using SkiaSharp;

namespace QuickSkin.Common.Services;

public static class ImageService
{
    /// <summary>
    ///     从指定路径加载图片，支持多种格式，异常时返回null
    /// </summary>
    /// <param name="path">图片文件路径</param>
    /// <returns>Bitmap对象</returns>
    public static Bitmap? LoadImage(string path)
    {
        if (!File.Exists(path))
            return null;

        try
        {
            using var stream = File.OpenRead(path);

            // SkiaSharp加载，兼容更多格式
            using var skia = SKBitmap.Decode(stream);

            if (skia == null)
                return null;

            using var image = SKImage.FromBitmap(skia);
            using var ms = new MemoryStream();
            image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(ms);
            ms.Position = 0;

            return new Bitmap(ms);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    ///     保存Bitmap到指定路径，自动识别格式（png、jpg、bmp等）
    /// </summary>
    /// <param name="bitmap">要保存的Bitmap对象</param>
    /// <param name="path">保存路径</param>
    public static void SaveImage(Bitmap bitmap, string path)
    {
        string ext = Path.GetExtension(path).ToLower();

        var format = ext switch
        {
            ".jpg" or ".jpeg" => SKEncodedImageFormat.Jpeg,
            ".bmp" => SKEncodedImageFormat.Bmp,
            ".webp" => SKEncodedImageFormat.Webp,
            _ => SKEncodedImageFormat.Png,
        };

        using var ms = new MemoryStream();
        bitmap.Save(ms);
        ms.Position = 0;
        using var skia = SKBitmap.Decode(ms);

        if (skia == null)
            return;

        using var image = SKImage.FromBitmap(skia);
        using var outStream = File.Open(path, FileMode.Create);
        using var data = image.Encode(format, 100);
        data.SaveTo(outStream);
    }

    /// <summary>
    ///     从文件系统打开图片
    /// </summary>
    /// <returns>获取的位图</returns>
    public static async Task<Bitmap?> OpenImageFile()
    {
        if (WindowManager.TopLevel == null)
            return null;

        var files = await WindowManager.TopLevel.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "选择图片",
                AllowMultiple = false,
                FileTypeFilter =
                [
                    new FilePickerFileType("图片文件")
                    {
                        Patterns = ["*.png", "*.jpg", "*.jpeg", "*.bmp"],
                    },
                ],
            }
        );

        if (files.Count <= 0)
            return null;

        try
        {
            var file = files[0];
            await using var stream = await file.OpenReadAsync();

            return new Bitmap(stream);
        }
        catch (Exception ex)
        {
            NotificationService.Show("坏欸", $"打开文件失败了！\n{ex.Message}", NotificationType.Error);

            return null;
        }
    }
}
