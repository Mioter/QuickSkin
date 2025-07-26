using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using QuickSkin.Common.Manager;
using QuickSkin.Common.Wrappers;
using QuickSkin.Core.ClassBase;
using QuickSkin.Models;
using QuickSkin.Views.Dialogs;

namespace QuickSkin.ViewModels.Dialogs;

public partial class NewCategoryItemViewModel : DataVerifyModelBase, IDialogContext
{
    private string? _iconKey;

    public NewCategoryItemViewModel()
    {
        InitialValidate();
    }

    public string Name
    {
        get;
        set
        {
            field = value;
            ValidateName(value);
        }
    } = "";

    public string? Description { get; set; }

    [ObservableProperty] public partial Geometry? Icon { get; set; }

    [ObservableProperty] public partial SolidColorBrush? IconBrush { get; set; }

    [RelayCommand]
    private async Task SetIcon()
    {
        if (WindowManager.TopLevel == null)
            return;

        var options = new ShowWindowOptions
        {
            Title = "图标选择",
            IsRestoreButtonVisible = false,
            IsFullScreenButtonVisible = false,
        };

        var model = new IconSelectorViewModel();
        _ = Dispatcher.UIThread.InvokeAsync(() => { model.InitializeResources(_iconKey); });
        bool result = await WindowBox.ShowDialog<IconSelector, bool>(model, options, WindowManager.TopLevel);

        if (!result || model.SelectedIconItem == null)
            return;

        Icon = model.SelectedIconItem.Geometry;
        _iconKey = model.SelectedIconItem.ResourceKey;
        IconBrush = model.SelectedIconBrush;
    }

    public CategoryItem GenerateCategoryItem()
    {
        return new CategoryItem
        {
            Id = $"Item_{DateTime.UtcNow:yyyyMMddHHmmssfff}",
            Name = Name,
            Description = Description,
            Icon = Icon,
            IconBrush = IconBrush,
            IconKey = _iconKey,
        };
    }

    #region 接口实现

    [RelayCommand]
    private void Ok()
    {
        Close(GenerateCategoryItem());
    }

    [RelayCommand]
    private void Cancel()
    {
        Close();
    }

    public void Close(object? result)
    {
        RequestClose?.Invoke(this, result);
    }

    public void Close()
    {
        RequestClose?.Invoke(this, null);
    }

    public event EventHandler<object?>? RequestClose;

    #endregion

    #region 数据校验

    private void ValidateName(string? value)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(value))
            errors.Add("名称不可以为空");

        SetErrors(nameof(Name), errors);
    }

    private void InitialValidate()
    {
        ValidateName(Name);
    }

    #endregion
}
