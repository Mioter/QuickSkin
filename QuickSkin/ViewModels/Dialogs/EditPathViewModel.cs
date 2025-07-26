using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using QuickSkin.Common.Manager;
using QuickSkin.Common.Utilities;
using QuickSkin.Core.ClassBase;
using QuickSkin.Core.DataAnnotations;

namespace QuickSkin.ViewModels.Dialogs;

public partial class EditPathViewModel : DataVerifyModelBase, IDialogContext
{
    public EditPathViewModel(string oldPath, string title = "标题未设置", params ValidationAttribute[] addAttributes)
    {
        OldPath = oldPath;
        Title = title;
        AddAttributes = addAttributes;

        InitialValidate();
    }

    public ValidationAttribute[] AddAttributes { get; }

    public string Title { get; }

    public string OldPath { get; }

    public string? NewPath
    {
        get;
        set
        {
            field = value;
            ValidatePath(value);
        }
    }

    [RelayCommand]
    private async Task SelectedPathAsync()
    {
        var topLevel = WindowManager.TopLevel;

        if (topLevel == null)
            return;

        string? selectedPath = await PathSelector.OpenFolderAsync(topLevel);

        if (selectedPath == null)
            return;

        NewPath = selectedPath;
    }

    #region 数据校验

    private void ValidatePath(string? value)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add("路径不能为空");
            SetErrors(nameof(NewPath), errors);

            return;
        }

        if (NewPath == OldPath)
        {
            errors.Add("新路径不能与旧路径相同！");
            SetErrors(nameof(NewPath), errors);

            return;
        }

        // 检查是否包含非法字符
        var isValidPathCharactersAttribute = new ValidPathCharactersAttribute();

        if (!isValidPathCharactersAttribute.IsValid(value))
        {
            errors.Add("路径包含非法字符！");
            SetErrors(nameof(NewPath), errors);

            return;
        }

        foreach (var attribute in AddAttributes)
        {
            if (attribute.IsValid(value))
                continue;

            errors.Add(attribute.ErrorMessage ?? "未设置错误消息！！！");
            SetErrors(nameof(NewPath), errors);

            return;
        }

        SetErrors(nameof(NewPath), errors);
    }

    private void InitialValidate()
    {
        ValidatePath(NewPath);
    }

    #endregion

    #region 接口实现

    [RelayCommand]
    private void Ok()
    {
        Close(NewPath);
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
}
