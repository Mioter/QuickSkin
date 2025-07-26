using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using QuickSkin.Common.Manager;
using QuickSkin.Core.ClassBase;
using QuickSkin.Core.Enums;
using QuickSkin.ViewModels;
using QuickSkin.ViewModels.Dialogs;
using QuickSkin.Views.Dialogs;
using Serilog;
using Ursa.Controls;

namespace QuickSkin.Views;

public partial class WorkWindow : InteractiveWindowBase
{
    private bool _isClosing;

    public WorkWindow()
    {
        InitializeComponent();
    }

    public override void CloseWindow()
    {
        _isClosing = true;
        Close();
    }

    protected override async void OnClosing(WindowClosingEventArgs e)
    {
        try
        {
            if (_isClosing)
                return;

            e.Cancel = true;

            await HandleWindowClosingAsync();
        }
        catch (Exception ex)
        {
            Log.Error("在关闭窗口时发生了错误: \n{Message}", ex.Message);
        }
    }

    private async Task HandleWindowClosingAsync()
    {
        var behavior = ConfigManager.ConfigModel.SystemConfig.ClosingBehavior;

        if (behavior == Core.Enums.ClosingBehavior.AskAbout)
        {
            behavior = await GetUserClosingBehaviorAsync();
        }

        switch (behavior)
        {
            case Core.Enums.ClosingBehavior.Exit:
                DoExit();

                break;
            case Core.Enums.ClosingBehavior.HideToTray:
                Hide();

                break;
            case Core.Enums.ClosingBehavior.AskAbout:
            default:
                // 其它情况无需处理
                break;
        }
    }

    private static async Task<ClosingBehavior> GetUserClosingBehaviorAsync()
    {
        var options = new OverlayDialogOptions
        {
            Mode = DialogMode.Question,
            CanDragMove = true,
            CanResize = false,
        };

        var model = new ExitConfirmViewModel();
        bool result = await OverlayDialog.ShowCustomModal<ExitConfirm, ExitConfirmViewModel, bool>(model, options: options);

        if (!result)
            return Core.Enums.ClosingBehavior.AskAbout;

        if (model.IsEnablePrompt)
            ConfigManager.ConfigModel.SystemConfig.ClosingBehavior = model.ClosingBehavior;

        return model.ClosingBehavior;
    }

    private void DoExit()
    {
        _isClosing = true;
        ApplicationViewModel.Shutdown();
        Close();
    }
}
