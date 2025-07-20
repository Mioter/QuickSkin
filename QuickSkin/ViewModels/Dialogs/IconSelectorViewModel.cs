using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using QuickSkin.Models;
using Semi.Avalonia;

namespace QuickSkin.ViewModels.Dialogs;

public partial class IconSelectorViewModel : ObservableObject, IDialogContext
{
    private readonly Icons? _resources = new();

    private readonly Dictionary<string, IconItem> _filledIcons = new();

    [ObservableProperty] public partial string? SearchText { get; set; }

    [ObservableProperty] public partial IconItem? SelectedIconItem { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedIconBrush))]
    public partial Color SelectedIconColor { get; set; } = Colors.DodgerBlue;

    public SolidColorBrush SelectedIconBrush => new(SelectedIconColor);

    public AvaloniaList<IconItem> FilteredStrokedIcons { get; set; } = [];

    public void InitializeResources()
    {
        if (_resources is null) return;

        foreach (var provider in _resources.MergedDictionaries)
        {
            if (provider is not ResourceDictionary dic) continue;

            foreach (object key in dic.Keys)
            {
                if (dic[key] is not Geometry geometry)
                    continue;

                if (key is not string keyStr)
                    continue;
                
                var icon = new IconItem
                {
                    ResourceKey = keyStr,
                    Geometry = geometry,
                };

                if (!keyStr.EndsWith("Stroked"))
                {
                    _filledIcons[keyStr.ToLowerInvariant()] = icon;
                }
            }
        }

        OnSearchTextChanged(string.Empty);
    }

    partial void OnSearchTextChanged(string? value)
    {
        string search = value?.ToLowerInvariant() ?? string.Empty;

        FilteredStrokedIcons.Clear();

        FilteredStrokedIcons.AddRange(_filledIcons
            .Where(i => i.Key.Contains(search))
            .Select(x => x.Value));
    }

    #region 接口实现

    [RelayCommand]
    private void Ok()
    {
        Close(true);
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
