using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;

namespace MklinkGUI;

public enum LinkMode
{
    FileMode,
    FolderMode
}

public class LinkModeToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not LinkMode mode)
            throw new InvalidOperationException("The target must be a LinkMode enum");

        return mode == LinkMode.FileMode; // 如果是 FileMode 则返回 true，否则返回 false
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool b)
            throw new InvalidOperationException("The value must be a boolean");

        return b ? LinkMode.FileMode : LinkMode.FolderMode; // 如果是 true 则返回 FileMode，否则返回 FolderMode
    }
}

public class LinkModeToBoolReverseConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not LinkMode mode)
            throw new InvalidOperationException("The target must be a LinkMode enum");

        return mode == LinkMode.FolderMode; // 如果是 FolderMode 则返回 true，否则返回 false
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool b)
            throw new InvalidOperationException("The value must be a boolean");

        return b ? LinkMode.FolderMode : LinkMode.FileMode; // 如果是 true 则返回 FolderMode，否则返回 FileMode
    }
}

public class LinkModeToHintStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not LinkMode mode)
            throw new InvalidOperationException("The target must be a LinkMode enum");
        return mode == LinkMode.FileMode ? "原始文件路径" : "原始文件夹路径";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new InvalidOperationException("Converting back from hint string to Link Mode is invalid!");
    }
}

public class MainWindowViewModel: INotifyPropertyChanged
{
    #region PrivateVariable

    private LinkMode _linkMode = LinkMode.FileMode; // true for file mode, false for folder mode.
    private string _linkPath = string.Empty;
    private string _targetPath = string.Empty;
    private string _executionResult = "等待执行";
    private Brush _executionResultForeground = new SolidColorBrush(Colors.White);
    private bool _isHardLink;
    private bool _isJunction;

    #endregion
    
    #region Property

    public LinkMode LinkMode
    {
        get => _linkMode;
        set
        {
            if (_linkMode == value) return;
            _linkMode = value;
            OnPropertyChanged(nameof(LinkMode));
        }
    }

    public string LinkPath
    {
        get => _linkPath;
        set
        {
            if (_linkPath == value) return;
            _linkPath = value;
            OnPropertyChanged(nameof(LinkPath));
        }
    }

    public string TargetPath
    {
        get => _targetPath;
        set
        {
            if (_targetPath == value) return;
            _targetPath = value;
            OnPropertyChanged(nameof(TargetPath));
        }
    }

    public bool IsHardLink
    {
        get => _isHardLink;
        set
        {
            if (_isHardLink == value) return;
            _isHardLink = value;
            OnPropertyChanged(nameof(IsHardLink));
        }
    }

    public bool IsJunction
    {
        get => _isJunction;
        set
        {
            if (_isJunction == value) return;
            _isJunction = value;
            OnPropertyChanged(nameof(IsJunction));
        }
    }

    public string ExecutionResult
    {
        get => _executionResult;
        set
        {
            if (_executionResult == value) return;
            _executionResult = value;
            OnPropertyChanged(nameof(ExecutionResult));
        }
    }

    public Brush ExecutionResultForeground
    {
        get => _executionResultForeground;
        set
        {
            if (_executionResultForeground == value) return;
            _executionResultForeground = value;
            OnPropertyChanged(nameof(ExecutionResultForeground));
        }
    }

    #endregion
    
    #region PropertyChangedHandler

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}