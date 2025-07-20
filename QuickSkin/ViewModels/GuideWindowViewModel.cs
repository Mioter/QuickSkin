using System.IO;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Input;
using QuickSkin.Common;
using QuickSkin.Common.Manager;
using QuickSkin.Common.Services;
using QuickSkin.Common.Services.DataBases;
using QuickSkin.Common.Wrappers;
using QuickSkin.Core.ClassBase;
using QuickSkin.Core.DataAnnotations;
using QuickSkin.Models;
using QuickSkin.ViewModels.Dialogs;
using QuickSkin.Views.Dialogs;
using Ursa.Controls;

namespace QuickSkin.ViewModels;

public partial class GuideWindowViewModel : ViewModelBase
{
    public GuideWindowViewModel()
    {
        using var repo = new WorkspaceInfoRepository(StaticConfig.DataBasePath);

        Workspaces.AddRange(repo.GetAll());
    }

    public string Title { get; } = $"{Workplace.CurrentAssemblyName} • 打开工作区";

    public AvaloniaList<WorkspaceInfo> Workspaces { get; } = [];

    [RelayCommand]
    private async Task NewWorkspace()
    {
        if (WindowManager.TopLevel == null)
            return;

        var options = new ShowWindowOptions
        {
            Title = $"{Workplace.CurrentAssemblyName} • 新建工作区",
            IsRestoreButtonVisible = false,
            IsFullScreenButtonVisible = false,
        };

        var windowBox = new WindowBox();
        var workspaceInfo = await windowBox.ShowDialog<WorkspaceInfo>(new NewWorkspace(), new NewWorkspaceViewModel(), options, WindowManager.TopLevel);

        if (workspaceInfo == null)
            return;

        using var repo = new WorkspaceInfoRepository(StaticConfig.DataBasePath);
        repo.Insert(workspaceInfo);
        Workspaces.Add(workspaceInfo);
    }

    [RelayCommand]
    private static void OpenWorkspace(WorkspaceInfo workspaceInfo)
    {
        Workplace.WorkspaceInfo = workspaceInfo;
        WindowManager.OpenWorkspace();
    }

    [RelayCommand]
    private static async Task EditWorkspaceName(WorkspaceInfo workspaceInfo)
    {
        var options = new OverlayDialogOptions
        {
            Title = "修改名称",
            Buttons = DialogButton.OKCancel,
            Mode = DialogMode.Info,
            CanDragMove = true,
        };

        string? result = await OverlayDialog.ShowCustomModal<EditText, EditTextViewModel, string>(new EditTextViewModel(workspaceInfo.Name, options.Title), options: options);

        if (string.IsNullOrEmpty(result))
            return;

        using (var repo = new WorkspaceInfoRepository(StaticConfig.DataBasePath))
        {
            repo.Update(workspaceInfo.Id, [nameof(workspaceInfo.Name)], [workspaceInfo.Name]);
        }

        workspaceInfo.Name = result;

        NotificationService.Show("好欸", $"修改{workspaceInfo.Name}的名称成功了", NotificationType.Success);
    }

    [RelayCommand]
    private static async Task EditWorkspaceInputPath(WorkspaceInfo workspaceInfo)
    {
        string path = workspaceInfo.InputPath ?? Path.Combine(StaticConfig.SourcePath, workspaceInfo.Name);

        var options = new OverlayDialogOptions
        {
            Title = "修改输入目录",
            Buttons = DialogButton.OKCancel,
            Mode = DialogMode.Info,
            CanDragMove = true,
        };

        string? result = await OverlayDialog.ShowCustomModal<EditPath, EditPathViewModel, string>(new EditPathViewModel(path, options.Title), options: options);

        if (string.IsNullOrEmpty(result))
            return;

        using (var repo = new WorkspaceInfoRepository(StaticConfig.DataBasePath))
        {
            repo.Update(workspaceInfo.Id, [nameof(workspaceInfo.InputPath)], [workspaceInfo.InputPath]);
        }

        workspaceInfo.InputPath = result;

        NotificationService.Show("好欸", $"修改{workspaceInfo.Name}的输入目录成功了", NotificationType.Success);
    }

    [RelayCommand]
    private static async Task EditWorkspaceOutPutPath(WorkspaceInfo workspaceInfo)
    {
        string path = workspaceInfo.InputPath ?? Path.Combine(StaticConfig.SourcePath, workspaceInfo.Name);

        var options = new OverlayDialogOptions
        {
            Title = "修改输出目录",
            Buttons = DialogButton.OKCancel,
            Mode = DialogMode.Info,
            CanDragMove = true,
        };

        string? result = await OverlayDialog.ShowCustomModal<EditPath, EditPathViewModel, string>(new EditPathViewModel(path, options.Title, new IsAbsolutePathAttribute
            {
                ErrorMessage = "路径必须为绝对文件夹",
            }, new ExistsDirectoryAttribute
            {
                ErrorMessage = "输出路径必需是存在的文件夹",
            },
            new OutputPathNotExistsAttribute
            {
                ErrorMessage = "该输出路径已存在于工作区数据库中，请更换！",
            }), options: options);

        if (string.IsNullOrEmpty(result))
            return;

        using (var repo = new WorkspaceInfoRepository(StaticConfig.DataBasePath))
        {
            repo.Update(workspaceInfo.Id, [nameof(workspaceInfo.OutputPath)], [workspaceInfo.OutputPath]);
        }

        workspaceInfo.OutputPath = result;
        NotificationService.Show("好欸", $"修改{workspaceInfo.Name}的输出目录成功了", NotificationType.Success);
    }

    [RelayCommand]
    private static async Task EditWorkspaceIcon(WorkspaceInfo workspaceInfo)
    {
        if (WindowManager.TopLevel == null)
            return;

        var options = new ShowWindowOptions
        {
            Title = "裁剪图片",
            IsRestoreButtonVisible = false,
            IsFullScreenButtonVisible = false,
        };

        var bitmap = await ImageService.OpenImageFile();

        if (bitmap == null)
            return;

        var windowBox = new WindowBox();
        var model = new ImageCroppingViewModel(bitmap);
        bool result = await windowBox.ShowDialog<ImageCropping, bool>(model, options, WindowManager.TopLevel);

        if (!result)
            return;

        string? oldIconId = workspaceInfo.IconId;
        workspaceInfo.Icon = model.CroppedImage;

        // 更新图标id
        if (oldIconId != workspaceInfo.IconId)
        {
            using var repo = new WorkspaceInfoRepository(StaticConfig.DataBasePath);
            repo.Update(workspaceInfo.Id, [nameof(workspaceInfo.IconId)], [workspaceInfo.IconId]);
        }

        NotificationService.Show("好欸", $"修改{workspaceInfo.Name}的图标成功了", NotificationType.Success);
    }

    [RelayCommand]
    private void DeleteWorkspace(WorkspaceInfo? workspaceInfo)
    {
        if (workspaceInfo == null)
            return;

        using (var repo = new WorkspaceInfoRepository(StaticConfig.DataBasePath))
        {
            repo.Delete(workspaceInfo.OutputPath);
        }

        Workspaces.Remove(workspaceInfo);

        // 清理图片缓存
        if (workspaceInfo.IconId != null)
            CacheManager.DeleteImage(workspaceInfo.IconId);

        NotificationService.Show("好欸", $"删除{workspaceInfo.Name}成功了", NotificationType.Success);
    }
}
