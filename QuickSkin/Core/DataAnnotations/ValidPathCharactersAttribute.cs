using System.ComponentModel.DataAnnotations;
using System.IO;

namespace QuickSkin.Core.DataAnnotations;

public class ValidPathCharactersAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is string path)
        {
            // 检查路径中是否包含非法字符
            return path.IndexOfAny(Path.GetInvalidPathChars()) < 0;
        }

        ErrorMessage = "值不是 string 类型！";

        return false;
    }
}
