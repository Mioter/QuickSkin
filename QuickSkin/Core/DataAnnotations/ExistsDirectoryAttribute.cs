using System.ComponentModel.DataAnnotations;
using System.IO;

namespace QuickSkin.Core.DataAnnotations;

public class ExistsDirectoryAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is string path)
        {
            return Directory.Exists(path);
        }

        ErrorMessage = "值不是 string 类型！";

        return false;
    }
}
