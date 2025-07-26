using System;
using System.IO;

namespace QuickSkin.Common;

public static class StaticConfig
{
    public static readonly string DataBasePath = Path.Combine(AppContext.BaseDirectory, "Data", "base.db");

    public static readonly string SourcePath = Path.Combine(AppContext.BaseDirectory, "Sources");

    public static readonly string ConfigPath = Path.Combine(AppContext.BaseDirectory, "Configs");

    public static readonly string LoggerPath = Path.Combine(AppContext.BaseDirectory, "Logs");

    public static string DataCategoryPath
    {
        get
        {
            ArgumentNullException.ThrowIfNull(Workplace.CurrentWorkspace);

            return Path.Combine(AppContext.BaseDirectory, "Data", $"{Workplace.CurrentWorkspace.Id}", "category.db");
        }
    }
}
