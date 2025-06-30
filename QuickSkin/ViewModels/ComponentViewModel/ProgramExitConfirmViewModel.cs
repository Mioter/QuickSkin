using CommunityToolkit.Mvvm.Input;
using QuickSkin.Definitions.Enums;
using Ursa.Controls;

namespace QuickSkin.ViewModels.ComponentViewModel;

public partial class ProgramExitConfirmViewModel(OverlayDialogOptions options) : DialogViewModelBase(options)
{
    public bool IsEnablePrompt { get; set; }

    public ClosingBehavior ClosingBehavior { get; set; }

    [RelayCommand]
    private void Hide()
    {
        ClosingBehavior = ClosingBehavior.HideToTray;
        Close(true);
    }

    [RelayCommand]
    private void Exit()
    {
        ClosingBehavior = ClosingBehavior.Exit;
        Close(true);
    }

    [RelayCommand]
    private void Cancel()
    {
        Close(false);
    }
}
