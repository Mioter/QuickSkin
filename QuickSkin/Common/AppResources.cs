using System.Collections.Generic;
using Avalonia.Media;
using QuickSkin.Common.Manager;
using QuickSkin.Common.Services;

namespace QuickSkin.Common;

public class AppResources
{
    public static readonly AppResources Default = new();

    public Dictionary<string, FontFamily> Fonts { get; set; } = new()
    {
        ["默认"] = "fonts:Inter#Inter,$Default",
        ["OPPOSans"] = "resm:QuickSkin.Assets.EmbeddedRes.Fonts.OPPOSans4.0.ttf#OPPO Sans4.0",
        ["汉仪唐美人"] = "resm:QuickSkin.Assets.EmbeddedRes.Fonts.HYTangBeauty.ttf#汉仪唐美人",
    };

    public string CurrentFont
    {
        get => ConfigManager.ConfigModel.UiConfig.CurrentFont;
        set
        {
            if (SetAppFont(Fonts[value]))
            {
                ConfigManager.ConfigModel.UiConfig.CurrentFont = value;
            }
        }
    }

    public void Initialize()
    {
        SetAppFont(Fonts[ConfigManager.ConfigModel.UiConfig.CurrentFont]);
    }

    public static bool SetAppFont(FontFamily font)
    {
        return ResourceAccessor.Set("AppFont", font);
    }
}
