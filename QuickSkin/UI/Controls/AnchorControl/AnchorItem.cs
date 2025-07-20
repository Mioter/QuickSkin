using Avalonia;
using Avalonia.Controls;

namespace QuickSkin.UI.Controls.AnchorControl;

public class AnchorItem : ContentControl
{
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<AnchorItem, object?>(nameof(Header));

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly StyledProperty<double> HeaderHeightProperty =
        AvaloniaProperty.Register<AnchorItem, double>(nameof(HeaderHeight), 40d, inherits: true);

    public double HeaderHeight
    {
        get => GetValue(HeaderHeightProperty);
        set => SetValue(HeaderHeightProperty, value);
    }
}
