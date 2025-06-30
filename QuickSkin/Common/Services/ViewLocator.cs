using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using QuickSkin.ViewModels;
using QuickSkin.ViewModels.PageModels;
using QuickSkin.Views;
using QuickSkin.Views.Pages;

namespace QuickSkin.Common.Services;

///<summary>
/// 视图定位器。
/// </summary>
public class ViewLocator : IDataTemplate
{
    public Control Build(object? param)
    {
        return param switch
        {
            GuideWindowViewModel => new GuideWindow(),
            MainViewModel => new MainView(),
            HomePageViewModel => new HomePage(),
            _ => throw new NotImplementedException(),
        };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
