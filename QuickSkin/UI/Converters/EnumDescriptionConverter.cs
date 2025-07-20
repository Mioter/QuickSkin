using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using QuickSkin.Core.Helper;

namespace QuickSkin.UI.Converters;

public class EnumDescriptionConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter != null)
        {
            value = parameter;
        }

        string? valueStr = value?.ToString();

        return valueStr == null ? "无法将传入的值或者参数转为'string'类型！" : EnumDescriptionStore.EnumDescriptions.GetValueOrDefault(valueStr, valueStr);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value;
    }
}
