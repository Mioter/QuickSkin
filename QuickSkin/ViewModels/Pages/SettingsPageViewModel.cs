using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using QuickSkin.Common;
using QuickSkin.Common.Manager;
using QuickSkin.Common.Utilities;
using QuickSkin.Core.ClassBase;
using QuickSkin.Core.DataAnnotations;
using QuickSkin.Core.Enums;
using QuickSkin.Core.Helper;
using QuickSkin.Models.Configs;

namespace QuickSkin.ViewModels.Pages;

public partial class SettingsPageViewModel : ViewModelBase
{
    public SettingsPageViewModel() => InitialValidate();
    
    public ConfigModel Config { get; } = ConfigManager.ConfigModel;

    public static AppResources AppResources => AppResources.Default;

    public static Dictionary<ClosingBehavior, string> ClosingBehaviors =>
        EnumHelper<ClosingBehavior>.GetValueDescriptionDictionary();
    
    public Dictionary<string, string> Themes { get; set; } = new()
    {
        ["Default"] = "跟随系统",
        ["Light"] = "亮色",
        ["Dark"] = "暗色",
    };

    public string? Use7ZipPathErrorText
    {
        get;
        set
        {
            field = value;

            if (value != null)
            {
                Config.ToolsConfig.Use7ZipCompression = false;
            }

            OnPropertyChanged();
        }
    }

    public string? Use7ZipPath
    {
        get => Config.ToolsConfig.Use7ZipPath;
        set
        {
            string? use7ZipPath = ValidateUse7ZipDllPath(value);

            Config.ToolsConfig.Use7ZipPath = use7ZipPath ?? value;
            OnPropertyChanged();
        }
    }
    
    [RelayCommand]
    private async Task SelectedUse7ZipDllPathAsync()
    {
        var topLevel = WindowManager.TopLevel;

        if (topLevel == null)
            return;

        string? selectedPath = await PathSelector.OpenFolderAsync(topLevel);

        if (selectedPath == null)
            return;

        Use7ZipPath = selectedPath;
        OnPropertyChanged(nameof(Use7ZipPath));
    }

    private void InitialValidate()
    {
        ValidateUse7ZipDllPath(Use7ZipPath);
    }
    
    public string? ValidateUse7ZipDllPath(string? value)
    {
        // 检查路径是否为空或空白
        if (string.IsNullOrWhiteSpace(value))
        {
            Use7ZipPathErrorText = "路径不能为空或空白字符串";

            return null;
        }

        var attribute = new AbsolutePathAttribute();

        if (!attribute.IsValid(value))
        {
            Use7ZipPathErrorText = "路径必须为绝对文件夹";
            return null;
        }

        if (IsFileNameWithExtension(value, "7z", ".dll", ".exe"))
        {
            // 检查文件是否存在
            if (File.Exists(value))
            {
                Use7ZipPathErrorText = null;
                return value;
            }

            Use7ZipPathErrorText = "文件不存在！";

            return null;
        }

        // 检查文件夹是否存在
        if (!Directory.Exists(value))
        {
            Use7ZipPathErrorText = "不存在的文件夹！";
            
            return null;
        }

        // 存在的文件夹
        string? user7Zip = FileFilter.FindFirstFile(value, 0, "7z.dll", "7z.exe");

        if (user7Zip == null)
        {
            Use7ZipPathErrorText = "文件夹中不存在 7z.dll 或者 7z.exe";
            
            return null;
        }

        Use7ZipPathErrorText = null;

        return user7Zip;
    }

    public static bool IsFileNameWithExtension(string filePath, string fileName, params string[] extensions)
    {
        // 获取文件名（包含扩展名）
        string actualFileName = Path.GetFileName(filePath);
    
        // 检查每个可能的文件名+扩展名组合
        return extensions.Select(extension => $"{fileName}{extension}")
            .Any(expectedFileName => string.Equals(actualFileName, expectedFileName, StringComparison.OrdinalIgnoreCase));
    }

}
