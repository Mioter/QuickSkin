namespace QuickSkin.Definitions.Enums;

/// <summary>
/// 关闭主窗口时的行为
/// </summary>
public enum ClosingBehavior
{
    /// <summary>
    /// 询问
    /// </summary>
    AskAbout,

    /// <summary>
    /// 直接退出
    /// </summary>
    Exit,

    /// <summary>
    /// 隐藏到系统托盘
    /// </summary>
    HideToTray,
}
