using CommunityToolkit.Mvvm.ComponentModel;

namespace QuickSkin.Models;

public partial class SelectedItem<TContent>(TContent content) : ObservableObject
{
    [ObservableProperty] public partial bool IsSelected { get; set; }

    [ObservableProperty] public partial TContent Content { get; set; } = content;
}
