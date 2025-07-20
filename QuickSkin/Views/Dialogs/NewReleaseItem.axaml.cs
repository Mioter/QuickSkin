using Avalonia.Controls;
using Avalonia.Interactivity;

namespace QuickSkin.Views.Dialogs;

public partial class NewReleaseItem : UserControl
{
    public NewReleaseItem()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        NameTextBlock.Focus();
        base.OnLoaded(e);
    }
}
