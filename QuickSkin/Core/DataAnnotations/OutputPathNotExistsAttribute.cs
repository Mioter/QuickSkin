using System.ComponentModel.DataAnnotations;
using QuickSkin.Common;
using QuickSkin.Common.Services.DataBases;

namespace QuickSkin.Core.DataAnnotations;

public class OutputPathNotExistsAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not string path)
        {
            ErrorMessage = "值不是 string 类型！";

            return true;
        }

        using var repo = new WorkspaceInfoRepository(StaticConfig.DataBasePath);

        return !repo.Exists(path);
    }
}
