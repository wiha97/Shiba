using System;
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
        Title = $"Shiba - {version.Substring(0,version.LastIndexOf('.'))}";
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
        FullString.Clear();
        HeadLabel.Content = "";
        ProfileBox.Items = Conf.Profiles;
        Probe.Start();
    }

    private void ProfileBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            Conf.LoadFile(ProfileBox.SelectedItem.ToString());
        }
        catch (Exception x)
        {
            // Error.Warning(x.Message);
            // Error.Log(x.ToString());
        }
    }
}