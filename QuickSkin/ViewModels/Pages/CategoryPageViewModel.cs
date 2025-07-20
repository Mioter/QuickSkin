using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using QuickSkin.Common;
using QuickSkin.Common.Manager;
using QuickSkin.Common.Services.DataBases;
using QuickSkin.Common.Utilities;
using QuickSkin.Common.Wrappers;
using QuickSkin.Core.ClassBase;
using QuickSkin.Models;
using QuickSkin.ViewModels.Dialogs;
using QuickSkin.Views.Dialogs;
using Ursa.Controls;

namespace QuickSkin.ViewModels.Pages;

public partial class CategoryPageViewModel : ViewModelBase
{
    public CategoryPageViewModel()
    {
        EnsureDefaultCategory();
        LoadCategoryItemsFromDb();
        CurrentCategoryItem = CategoryItems[0];
    }

    private readonly CategoryItem _defaultCategory = new()
    {
        Id = "Item_Default",
        Name = "未分类",
    };

    private void EnsureDefaultCategory()
    {
        using var repo = new CategoryMapRepository(StaticConfig.DataCategoryPath);
        
        if (!repo.Exists(_defaultCategory.Id))
        {
            repo.Insert(_defaultCategory);

            // 创建category.db中的表
            using (new ReleaseItemRepository(_defaultCategory.Id, StaticConfig.DataCategoryPath)) { }
        }
        else
        {
            // 更新分类信息
            repo.Update(_defaultCategory);

            // 确保category.db中有对应表
            string? tableName = repo.GetIdByCategoryName(_defaultCategory.Name);

            if (string.IsNullOrEmpty(tableName)) 
                return;

            // 若表不存在则创建
            using (new ReleaseItemRepository(tableName, StaticConfig.DataCategoryPath)) { }
        }
    }

    private void LoadCategoryItemsFromDb()
    {
        using var repo = new CategoryMapRepository(StaticConfig.DataCategoryPath);
        var items = repo.GetAll();

        CategoryItems.AddRange(items);
    }

    public AvaloniaList<CategoryItem> CategoryItems { get; } = [];

    public CategoryItem CurrentCategoryItem
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                // 异步加载，避免UI卡顿
                _ = Task.Run(() => LoadReleaseItems(value));
            }
        }
    }

    public AvaloniaList<ReleaseItemModel> ReleaseItems { get; set; } = [];

    private void LoadReleaseItems(CategoryItem value)
    {
        using var repo = new CategoryMapRepository(StaticConfig.DataCategoryPath);

        // 获取表名
        string? tableName = repo.GetIdByCategoryName(value.Name);

        if (string.IsNullOrEmpty(tableName)) return;

        using var releaseItemRepository = new ReleaseItemRepository(tableName, StaticConfig.DataCategoryPath);
        var items = releaseItemRepository.GetAll();

        // UI线程更新集合
        Dispatcher.UIThread.Post(() =>
        {
            ReleaseItems.Clear();
            ReleaseItems.AddRange(items);
        });
    }

    [RelayCommand]
    private async Task AddCategoryItem()
    {
        var options = new OverlayDialogOptions
        {
            CanDragMove = true,
            CanResize = false,
        };

        var categoryItem = await OverlayDialog.ShowCustomModal<NewCategoryItem, NewCategoryItemViewModel, CategoryItem>(
            new NewCategoryItemViewModel(),
            options: options
        );

        if (categoryItem == null)
            return;

        // 保存到category.db的CategoryMap表
        using (var repo = new CategoryMapRepository(StaticConfig.DataCategoryPath))
        {
            repo.Insert(categoryItem);
        }

        // 在category.db中创建表（表结构同ReleaseItemRepository）
        using (new ReleaseItemRepository(categoryItem.Id, StaticConfig.DataCategoryPath)) { }

        CategoryItems.Add(categoryItem);
    }

    [RelayCommand]
    private static void EditCategoryItem(CategoryItem item) { }

    [RelayCommand]
    private void DeleteCategoryItem(CategoryItem item)
    {
        using var repo = new CategoryMapRepository(StaticConfig.DataCategoryPath);

        // 1. 获取表名
        string? tableName = repo.GetIdByCategoryName(item.Name);

        if (!string.IsNullOrEmpty(tableName))
        {
            // 2. 级联删除ReleaseItem相关图片资源
            using (var releaseRepo = new ReleaseItemRepository(tableName, StaticConfig.DataCategoryPath))
            {
                var allItems = releaseRepo.GetAll();
                var ids = allItems.Select(x => x.IconId).OfType<string>();

                CacheManager.DeleteImages(ids);
            }

            // 3. 删除category.db中的表
            using var db = new DatabaseService(StaticConfig.DataCategoryPath);
            db.DropTable(tableName);
        }

        // 4. 删除映射
        repo.Delete(item.Id);

        if (item.Id == CurrentCategoryItem.Id)
        {
            if (repo.Get(_defaultCategory.Id) is { } categoryItem)
                CurrentCategoryItem = categoryItem;
        }

        // 5. 移除内存对象
        CategoryItems.Remove(item);
    }

    [RelayCommand]
    private async Task AddNewReleaseItem(CategoryItem? item)
    {
        var options = new OverlayDialogOptions
        {
            CanDragMove = true,
            CanResize = false,
        };

        var releaseItem = await OverlayDialog.ShowCustomModal<NewReleaseItem, NewReleaseItemViewModel, ReleaseItemModel>(
            new NewReleaseItemViewModel(),
            options: options
        );

        if (releaseItem == null)
            return;

        var categoryItem = item ?? CurrentCategoryItem;

        using (var repo = new CategoryMapRepository(StaticConfig.DataCategoryPath))
        {
            // 获取表名
            string? tableName = repo.GetIdByCategoryName(categoryItem.Name);

            if (tableName != null)
            {
                using var releaseItemRepository = new ReleaseItemRepository(tableName, StaticConfig.DataCategoryPath);
                releaseItemRepository.Insert(releaseItem);
            }
        }

        ReleaseItems.Add(releaseItem);
    }

    [RelayCommand]
    private static void EditReleaseItem(ReleaseItemModel item) { }

    [RelayCommand]
    private void DeleteReleaseItem(ReleaseItemModel item)
    {
        using (var repo = new CategoryMapRepository(StaticConfig.DataCategoryPath))
        {
            // 1. 获取表名
            string? tableName = repo.GetIdByCategoryName(item.Name);

            if (!string.IsNullOrEmpty(tableName))
            {
                // 2. 删除对应项目的图片资源
                if (item.IconId != null)
                {
                    CacheManager.DeleteImage(item.IconId);
                }

                // 3. 删除对应表中的项目
                using var releaseItemRepository = new ReleaseItemRepository(tableName, StaticConfig.DataCategoryPath);
                releaseItemRepository.Delete(item.Name);
            }
        }

        ReleaseItems.Remove(item);
    }

    [RelayCommand]
    private static void OpenReleaseItem(ReleaseItemModel item) { }

    [RelayCommand]
    private void SetCurrentCategoryItem(CategoryItem item)
    {
        CurrentCategoryItem = item;
    }

    [RelayCommand]
    public async Task DropReleaseItem(object tuple)
    {
        if (tuple is not (DragEventArgs e, ReleaseItemModel model)) return;

        var storageItems = e.Data.GetFiles();

        if (storageItems == null || WindowManager.TopLevel == null)
            return;

        var options = new ShowWindowOptions
        {
            Title = $"{Workplace.CurrentAssemblyName} • 选择项目",
            IsFullScreenButtonVisible = false,
            CanResize = true,
            SizeToContent = SizeToContent.Manual,
            MinHeight = 400,
            MinWidth = 600,
        };

        var allFilePaths = PathExtractor.GetAllFilePaths(storageItems);
        var zipFiles = FileFilter.FilterByExtension(allFilePaths, ".zip", ".rar", ".7z");

        var windowBox = new WindowBox();
        var vm = new ArchiveSelectorViewModel();
        vm.AllFileItems.AddRange(zipFiles.Select(x => new SelectedItem(x)));
        var selectedFileList = await windowBox.ShowDialog<ArchiveSelector, AvaloniaList<string>>(vm, options, WindowManager.TopLevel);

        if (selectedFileList != null)
        {
        }
    }
}
