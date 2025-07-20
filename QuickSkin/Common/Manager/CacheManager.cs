using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Media.Imaging;
using QuickSkin.Common.Services.DataBases;
using QuickSkin.Common.Utilities;

namespace QuickSkin.Common.Manager;

public static class CacheManager
{
    public static Bitmap NotExist { get; } = GetDefaultCover();

    public static Bitmap Loading { get; } = GetDefaultCover();

    public static WeakCache<string, Bitmap> ImageCache { get; } = new();

    /// <summary>
    ///     设置图片到缓存和数据库。
    /// </summary>
    public static void SetImage(string id, Bitmap bitmap)
    {
        ImageCache[id] = bitmap;
        using var repo = new ImageRepository(StaticConfig.DataBasePath);
        repo.SaveImage(id, bitmap);
    }

    /// <summary>
    ///     通过图片Id删除图片，先从数据库删除，然后从缓存删除。
    /// </summary>
    public static void DeleteImage(string id)
    {
        using var repo = new ImageRepository(StaticConfig.DataBasePath);
        repo.DeleteImage(id);
        ImageCache.Remove(id);
    }
    
    /// <summary>
    ///     通过图片Id集合批量删除图片，先从数据库删除，然后从缓存删除。
    /// </summary>
    public static void DeleteImages(IEnumerable<string> ids)
    {
        using var repo = new ImageRepository(StaticConfig.DataBasePath);

        foreach (string id in ids)
        {
            repo.DeleteImage(id);
            ImageCache.Remove(id);
        }
    }
    
    /// <summary>
    ///     获取默认图片
    /// </summary>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException">无法找到默认封面资源时抛出异常</exception>
    private static Bitmap GetDefaultCover()
    {
        try
        {
            var assembly = Workplace.CurrentAssembly;

            using var stream =
                assembly.GetManifestResourceStream("QuickSkin.Assets.EmbeddedRes.Images.没有图片哦.webp")
             ?? throw new FileNotFoundException("无法找到默认图片资源");

            return new Bitmap(stream);
        }
        catch (Exception)
        {
            // 如果资源加载失败，返回一个空位图
            var bitmap = new RenderTargetBitmap(new PixelSize(100, 100));

            return bitmap;
        }
    }
}
