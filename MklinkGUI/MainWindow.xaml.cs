using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

namespace MklinkGUI;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly MainWindowViewModel _mainWindowViewModel;

    public MainWindow()
    {
        InitializeComponent();

        _mainWindowViewModel = (DataContext as MainWindowViewModel)!;

        CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand,
            (_, _) => SystemCommands.CloseWindow(this)));
        CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand,
            (_, _) => SystemCommands.MinimizeWindow(this)));
    }

    #region PrivateMethod

    private void SelectLinkPathButton(object sender, RoutedEventArgs eventArgs)
    {
        FolderBrowserDialog dialog = new();
        if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
        _mainWindowViewModel.LinkPath = dialog.SelectedPath;
    }

    private void SelectTargetPathButton(object sender, RoutedEventArgs eventArgs)
    {
        if (_mainWindowViewModel.LinkMode == LinkMode.FileMode)
        {
            OpenFileDialog dialog = new()
            {
                Multiselect = false
            };
            if (dialog.ShowDialog() != true) return;
            _mainWindowViewModel.TargetPath = dialog.FileName;
        }
        else
        {
            FolderBrowserDialog dialog = new();
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            _mainWindowViewModel.TargetPath = dialog.SelectedPath;
        }
    }

    private async void ExecuteMklinkButton(object sender, RoutedEventArgs eventArgs)
    {
        if (_mainWindowViewModel.LinkPath == string.Empty || _mainWindowViewModel.TargetPath == string.Empty) return;
        string param;
        if (_mainWindowViewModel.LinkMode == LinkMode.FileMode)
            param = _mainWindowViewModel.IsHardLink == false ? "" : "/H";
        else
            param = _mainWindowViewModel.IsJunction == false ? "/D" : "/J";

        DirectoryInfo targetPathDirectoryInfo = new(_mainWindowViewModel.TargetPath);
        string fullLinkPath = _mainWindowViewModel.LinkPath + "\\" + targetPathDirectoryInfo.Name;
        string command = $"/c \"mklink {param} {fullLinkPath} {_mainWindowViewModel.TargetPath}\"";

        ProcessStartInfo info = new()
        {
            FileName = "cmd.exe",
            Arguments = command,
            UseShellExecute = false,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        StringBuilder error = new();

        Process process = new();
        process.StartInfo = info;
        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null)
                error.AppendLine(e.Data);
        };

        process.Start();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();
        if (error.ToString() == string.Empty)
        {
            _mainWindowViewModel.ExecutionResult = "成功执行";
            _mainWindowViewModel.ExecutionResultForeground = new SolidColorBrush(Colors.Chartreuse);
        }
        else
        {
            _mainWindowViewModel.ExecutionResult = "执行失败，错误输出如下\n" + error;
            _mainWindowViewModel.ExecutionResultForeground = new SolidColorBrush(Colors.Red);
        }
    }

    #endregion
}