using CommunityToolkit.Mvvm.ComponentModel;

namespace QuickSkin.Models.Configs;

public partial class UiConfig : ObservableObject
{
    [ObservableProperty] public partial string Theme { get; set; } = "Default";

    // 已加载的资源字典列表
    [ObservableProperty] public partial string CurrentFont { get; set; } = "默认";
}
