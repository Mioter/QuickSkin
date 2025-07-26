using CommunityToolkit.Mvvm.ComponentModel;

namespace QuickSkin.Models.Configs;

public partial class ToolsConfig : ObservableObject
{
    [ObservableProperty]
    public partial bool Use7ZipCompression { get; set; }
    
    public string? Use7ZipPath { get; set; } 

}
