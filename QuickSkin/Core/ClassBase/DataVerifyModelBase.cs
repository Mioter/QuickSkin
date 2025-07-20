using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace QuickSkin.Core.ClassBase;

public class DataVerifyModelBase : ObservableObject, INotifyDataErrorInfo
{
    // 错误信息存储
    private readonly Dictionary<string, List<string>> _errors = new();

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public bool HasErrors => _errors.Count != 0;

    public IEnumerable GetErrors(string? propertyName)
    {
        if (propertyName != null && _errors.TryGetValue(propertyName, out var errors))
            return errors;

        return Enumerable.Empty<string>();
    }

    private void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    protected private void SetErrors(string propertyName, List<string> errors)
    {
        if (errors.Count != 0)
        {
            _errors[propertyName] = errors;
        }
        else
        {
            _errors.Remove(propertyName);
        }

        OnErrorsChanged(propertyName);
        OnPropertyChanged(nameof(HasErrors));
    }
}
