using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace QuickSkin.Core.DataAnnotations;

/// <summary>
/// 验证文件路径是否存在
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class FileExistsAttribute : ValidationAttribute
{
    /// <summary>
    ///     验证文件或目录是否存在
    /// </summary>
    public override bool IsValid(object? value)
    {
        // 允许空值（可以结合Required特性使用）
        if (value is not string path)
        {
            ErrorMessage = "值不是 string 类型！";

            return false;
        }

        try
        {
            // 检查文件是否存在
            return File.Exists(path);
        }
        catch (Exception)
        {
            return false;
        }
    }
}