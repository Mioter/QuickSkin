using System;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using QuickSkin.Common;
using QuickSkin.Common.Manager;
using QuickSkin.Common.Services.DataBases;
using QuickSkin.Core.Enums;

namespace QuickSkin.Models;

public partial class WorkspaceInfo : ObservableObject
{
    private LoadingState _loadingState;

    public required string Id { get; set; }

    [ObservableProperty] public required partial string Name { get; set; }

    [ObservableProperty] public partial string? InputPath { get; set; }

    [ObservableProperty] public required partial string OutputPath { get; set; }

    [ObservableProperty] public partial WorkingMode WorkingMode { get; set; }

    public string? IconId { get; internal set; }

    public Bitmap? Icon
    {
        get
        {
            // 如果封面路径不存在，返回不存在封面
            if (string.IsNullOrEmpty(IconId) || _loadingState == LoadingState.NotExist)
                return CacheManager.NotExist;

            // 如果正在加载中，返回加载中封面
            if (_loadingState == LoadingState.Loading)
                return CacheManager.Loading;

            // 尝试从缓存获取图片
            if (CacheManager.ImageCache.TryGetValue(IconId, out var bitmap) && bitmap != null)
            {
                _loadingState = LoadingState.Loaded;

                return bitmap;
            }

            // 缓存未命中，标记为正在加载
            _loadingState = LoadingState.Loading;

            // 启动异步加载任务
            _ = Task.Run(() =>
            {
                using var repo = new ImageRepository(StaticConfig.DataBasePath);
                var dbBitmap = repo.LoadImage(IconId);

                if (dbBitmap == null)
                {
                    _loadingState = LoadingState.NotExist;

                    return;
                }

                CacheManager.ImageCache.Add(IconId, dbBitmap);
                _loadingState = LoadingState.Loaded;
                OnPropertyChanged(); // 通知 UI 更新
            });

            // 首次或加载中时返回加载中封面
            return CacheManager.Loading;
        }
        set
        {
            if (value != null)
            {
                IconId ??= Guid.NewGuid().ToString();

                _ = Task.Run(() =>
                {
                    CacheManager.SetImage(IconId, value);
                    OnPropertyChanged();
                });
            }
            else
            {
                if (IconId == null)
                    return;

                _ = Task.Run(() => CacheManager.DeleteImage(IconId));
                IconId = null;

                OnPropertyChanged();
            }
        }
    }
}
