using System.Collections.ObjectModel;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using QuickSkin.Common.Utilities;
using QuickSkin.Models;

namespace QuickSkin.ViewModels.PageModels;

public partial class HomePageViewModel : ViewModelBase
{
    public static IBrush RandomColor => ColorGenerator.GeneratePastelColor();

    public ObservableCollection<ReleaseItemModel> ReleaseItems { get; } = [];

    [RelayCommand]
    private static void CreateNewReleaseItem() { }

    [RelayCommand]
    private static void EditReleaseItem(ReleaseItemModel item) { }

    [RelayCommand]
    private static void DeleteReleaseItem(ReleaseItemModel item) { }

    [RelayCommand]
    private static void OpenReleaseItem(ReleaseItemModel item) { }
}
