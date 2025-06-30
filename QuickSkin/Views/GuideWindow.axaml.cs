using System;
using QuickSkin.Definitions.ClassBase;
using QuickSkin.ViewModels;

namespace QuickSkin.Views;

public partial class GuideWindow : InteractiveWindowBase
{
    private bool _isWork;

    public GuideWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        if (_isWork)
            return;

        ApplicationViewModel.Shutdown();
    }

    public override void CloseWindow()
    {
        _isWork = true;
        Close();
    }
}
