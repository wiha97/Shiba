using System.Reflection;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Shiba.Magic;
using Shiba.Views;

namespace Shiba;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Conf.Win = this;
        Conf.FirstLaunch();
        string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        Title = $"Shiba - {version}";
        if (Conf.Profile.File != null)
        {
            Conf.Modify = true;
            ProfileBtn.Content = "Modify Profile";
        }
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        new ConfWin().Show();
    }

    private void DoneBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        FileManager.MoveFile($"{Conf.SourceDir}/{Conf.SourceFile}");
        Close();
    }

    private void RefreshBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Stacky.Children.Clear();
        FullString.Clear();
        HeadLabel.Content = "";
        Probe.Start();
    }
}