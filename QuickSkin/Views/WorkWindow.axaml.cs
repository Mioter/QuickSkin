using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using QuickSkin.Common.Manager;
using QuickSkin.Definitions.ClassBase;
using QuickSkin.ViewModels;
using QuickSkin.ViewModels.ComponentViewModel;
using QuickSkin.Views.Components;
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
        var behavior = ConfigManager.ConfigModel.ClosingBehavior;

        if (behavior == Definitions.Enums.ClosingBehavior.AskAbout)
        {
            behavior = await GetUserClosingBehaviorAsync();
        }

        switch (behavior)
        {
            case Definitions.Enums.ClosingBehavior.Exit:
                DoExit();
                break;
            case Definitions.Enums.ClosingBehavior.HideToTray:
                Hide();
                break;
            case Definitions.Enums.ClosingBehavior.AskAbout:
            default:
                // 其它情况无需处理
                break;
        }
    }

    private static async Task<Definitions.Enums.ClosingBehavior> GetUserClosingBehaviorAsync()
    {
        var options = new OverlayDialogOptions
        {
            Title = "退出提示",
            Mode = DialogMode.Question,
            CanDragMove = true,
            CanResize = false,
        };

        var model = new ProgramExitConfirmViewModel(options);
        await OverlayDialog.ShowCustomModal<ExitConfirm, ProgramExitConfirmViewModel, object>(model, options: options);

        if (model.IsCancel)
            return Definitions.Enums.ClosingBehavior.AskAbout;

        if (model.IsEnablePrompt)
            ConfigManager.ConfigModel.ClosingBehavior = model.ClosingBehavior;

        return model.ClosingBehavior;
    }

    private void DoExit()
    {
        _isClosing = true;
        ApplicationViewModel.Shutdown();
        Close();
    }
}
