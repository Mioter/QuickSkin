using System.Reflection;
using QuickSkin.Models;

namespace QuickSkin.Common;

public static class Workplace
{
    public static Assembly CurrentAssembly { get; } = Assembly.GetExecutingAssembly();

    public static string CurrentAssemblyName => CurrentAssembly.GetName().Name ?? "QuickSkin";

    public static WorkspaceInfo? CurrentWorkspace { get; set; }
}
