using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using QuickSkin.Common.Manager;
using QuickSkin.Common.Services;
using QuickSkin.Common.Wrappers;
using QuickSkin.Core.ClassBase;
using QuickSkin.Models;
using QuickSkin.Views.Dialogs;

namespace QuickSkin.ViewModels.Dialogs;

public partial class NewReleaseItemViewModel : DataVerifyModelBase, IDialogContext
{
    public NewReleaseItemViewModel()
    {
        InitialValidate();
    }

    [ObservableProperty] public partial Bitmap? Icon { get; set; }

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

    [RelayCommand]
    private async Task SetCover()
    {
        if (WindowManager.TopLevel == null)
            return;

        var options = new ShowWindowOptions
        {
            Title = "裁剪图片",
            IsRestoreButtonVisible = false,
            IsFullScreenButtonVisible = false,
        };

        var bitmap = await ImageService.OpenImageFile();

        if (bitmap == null)
            return;

        var windowBox = new WindowBox();
        var model = new ImageCroppingViewModel(bitmap);
        bool result = await windowBox.ShowDialog<ImageCropping, bool>(model, options, WindowManager.TopLevel);

        if (!result)
            return;

        Icon = model.CroppedImage;
    }

    public ReleaseItemModel GenerateReleaseItem()
    {
        return new ReleaseItemModel
        {
            Name = Name,
            Description = Description,
            Category = null,
            Icon = Icon,
        };
    }

    #region 接口实现

    [RelayCommand]
    private void Ok()
    {
        Close(GenerateReleaseItem());
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
