using System.Collections.Generic;
using QuickSkin.Common.Manager;
using QuickSkin.Core.ClassBase;
using QuickSkin.Core.Enums;
using QuickSkin.Core.Helper;
using QuickSkin.Models;

namespace QuickSkin.ViewModels.Pages;

public class SettingsPageViewModel : ViewModelBase
{
    public ConfigModel Config { get; } = ConfigManager.ConfigModel;

    public static Dictionary<ClosingBehavior, string> ClosingBehaviors =>
        EnumHelper<ClosingBehavior>.GetValueDescriptionDictionary();
}
