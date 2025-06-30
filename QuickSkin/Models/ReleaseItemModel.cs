using Avalonia.Media.Imaging;

namespace QuickSkin.Models;

public class ReleaseItemModel(Bitmap cover, string name, string description, string[] tags)
{
    public Bitmap Cover { get; set; } = cover;

    public string Name { get; set; } = name;

    public string Description { get; set; } = description;

    public string[] Tags { get; set; } = tags;
}
