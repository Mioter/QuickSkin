using System;
using System.Linq;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using QuickSkin.Models;

namespace QuickSkin.ViewModels.Dialogs;

public partial class ArchiveSelectorViewModel : ObservableObject, IDialogContext
{
    public AvaloniaList<SelectedItem> SelectedFileItems { get; set; } = [];
    
    public AvaloniaList<SelectedItem> AllFileItems { get; set; } = [];

    public bool IsSelectAll
    {
        get;
        set
        {
            field = value;
            
            if (value)
            {
                // 将筛选结果添加到 SelectedFileItems
                SelectedFileItems.AddRange(AllFileItems
                    .Where(item => item is { IsSelected: false })); // 条件筛选
            }
            else
            {
                SelectedFileItems.Clear();
            }
            
            OnPropertyChanged();
        }
    }

    [RelayCommand]
    private void SelectFile(SelectedItem item)
    {

        if (!SelectedFileItems.Remove(item))
        {
            SelectedFileItems.Add(item);
        }
    }

    [RelayCommand]
    private void DeleteFile(SelectedItem item)
    {
        SelectedFileItems.Remove(item);
    }
    
    #region 接口实现

    [RelayCommand]
    private void Ok()
    {
        Close(SelectedFileItems.Where(item => item is { Content: string }) // 条件筛选
            .Select(item => item.Content));
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
