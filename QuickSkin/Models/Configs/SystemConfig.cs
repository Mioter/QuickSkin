using QuickSkin.Core.Enums;

namespace QuickSkin.Models.Configs;

public class SystemConfig
{
    public ClosingBehavior ClosingBehavior { get; set; } = ClosingBehavior.AskAbout;
}
