using CommunityToolkit.Mvvm.ComponentModel;

namespace QuickSkin.Models;

public partial class SelectedItem(object content) : ObservableObject
{
    [ObservableProperty]
    public partial bool IsSelected { get; set; }
    
    [ObservableProperty]
    public partial object Content { get; set; } = content;
}
