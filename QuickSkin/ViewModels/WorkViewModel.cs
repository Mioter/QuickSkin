using CommunityToolkit.Mvvm.ComponentModel;
using QuickSkin.Core.ClassBase;

namespace QuickSkin.ViewModels;

public partial class WorkViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NavigationWidth))]
    public partial bool IsNavigationExpand { get; set; }

    public double NavigationWidth => IsNavigationExpand ? 130 : 55;
}
