using Avalonia.Media;

namespace QuickSkin.Models;

public class CategoryItem
{
    public required string Id { get; init; }

    public string? IconKey { get; internal init; }

    public Geometry? Icon { get; set; }

    public SolidColorBrush? IconBrush { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }
}
