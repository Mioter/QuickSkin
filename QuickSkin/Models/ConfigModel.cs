using QuickSkin.Core.Enums;

namespace QuickSkin.Models;

public class ConfigModel
{
    public ClosingBehavior ClosingBehavior { get; set; } = ClosingBehavior.AskAbout;
}
