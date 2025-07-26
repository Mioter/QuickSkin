using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using QuickSkin.Common;
using QuickSkin.Common.Manager;
using QuickSkin.Common.Services;
using QuickSkin.Common.Utilities;
using QuickSkin.Common.Wrappers;
using QuickSkin.Core.ClassBase;
using QuickSkin.Core.DataAnnotations;
using QuickSkin.Core.Enums;
using QuickSkin.Core.Helper;
using QuickSkin.Models;
using QuickSkin.Views.Dialogs;

namespace QuickSkin.ViewModels.Dialogs;

public partial class NewWorkspaceViewModel : DataVerifyModelBase, IDialogContext
{
    private bool _userHasSelectedInputDirectory;

    public NewWorkspaceViewModel()
    {
        InitialValidate();
    }

    public string Name
    {
        get;
        set
        {
            SetProperty(ref field, value);

            if (!_userHasSelectedInputDirectory)
                OnPropertyChanged(nameof(InputPath));

            ValidateName(value);
        }
    } = "";

    public string? InputPath
    {
        get => _userHasSelectedInputDirectory ? field : Path.Combine(StaticConfig.SourcePath, Name);
        set
        {
            SetProperty(ref field, value);
            _userHasSelectedInputDirectory = true;
            ValidateInputPathBase(value);
        }
    }

    public string OutputPath
    {
        get;
        set
        {
            SetProperty(ref field, value);
            ValidateOutputPathBase(value);
        }
    } = "";

    [ObservableProperty] public partial WorkingMode WorkingMode { get; set; }

    [ObservableProperty] public partial Bitmap? Icon { get; set; }

    public Dictionary<WorkingMode, string> WorkingModes { get; } =
        EnumHelper<WorkingMode>.GetValueDescriptionDictionary();

    [RelayCommand]
    private async Task SelectedInputPathAsync()
    {
        var topLevel = WindowManager.TopLevel;

        if (topLevel == null)
            return;

        InputPath = await PathSelector.OpenFolderAsync(topLevel);
    }

    [RelayCommand]
    private async Task SelectedOutputPathAsync()
    {
        var topLevel = WindowManager.TopLevel;

        if (topLevel == null)
            return;

        string? selectedPath = await PathSelector.OpenFolderAsync(topLevel);

        if (selectedPath == null)
            return;

        OutputPath = selectedPath;
    }

    [RelayCommand]
    private async Task SetIcon()
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

        var model = new ImageCroppingViewModel(bitmap);
        bool result = await WindowBox.ShowDialog<ImageCropping, bool>(model, options, WindowManager.TopLevel);

        if (!result)
            return;

        Icon = model.CroppedImage;
    }

    [RelayCommand]
    private void Cancel()
    {
        Close();
    }

    [RelayCommand]
    private void Ok()
    {
        Close(GenerateWorkspaceInfo());
    }

    public WorkspaceInfo GenerateWorkspaceInfo()
    {
        return new WorkspaceInfo
        {
            Id = $"Workspace • {DateTime.UtcNow:yyyyMMddHHmmssfff}",
            Name = Name,
            InputPath = _userHasSelectedInputDirectory ? InputPath : null,
            OutputPath = OutputPath,
            WorkingMode = WorkingMode,
            Icon = Icon,
        };
    }

    #region 接口实现

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

    private void ValidateInputPathBase(string? value)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add("输入路径不能为空");
            SetErrors(nameof(InputPath), errors);

            return;
        }

        var isAbsolutePathAttribute = new AbsolutePathAttribute();

        if (!isAbsolutePathAttribute.IsValid(value))
        {
            errors.Add("路径必须为绝对文件夹");
            SetErrors(nameof(OutputPath), errors);

            return;
        }

        var existsDirectoryAttribute = new DirectoryExistsAttribute();

        if (!existsDirectoryAttribute.IsValid(value))
        {
            errors.Add("路径必须为存在的文件夹");
        }

        SetErrors(nameof(InputPath), errors);
    }

    private void ValidateOutputPathBase(string? value)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add("输出路径不能为空");
            SetErrors(nameof(OutputPath), errors);

            return;
        }

        var isAbsolutePathAttribute = new AbsolutePathAttribute();

        if (!isAbsolutePathAttribute.IsValid(value))
        {
            errors.Add("路径必须为绝对文件夹");
            SetErrors(nameof(OutputPath), errors);

            return;
        }

        var existsDirectoryAttribute = new DirectoryExistsAttribute();

        if (!existsDirectoryAttribute.IsValid(value))
        {
            errors.Add("路径必须为存在的文件夹");
            SetErrors(nameof(OutputPath), errors);

            return;
        }

        // 检查数据库中是否已存在
        var attribute = new OutputPathNotExistsAttribute();

        if (!attribute.IsValid(value))
        {
            errors.Add("该输出路径已存在于工作区数据库中，请更换！");
        }

        SetErrors(nameof(OutputPath), errors);
    }

    private void InitialValidate()
    {
        ValidateName(Name);
        ValidateOutputPathBase(OutputPath);
    }

    #endregion
}
