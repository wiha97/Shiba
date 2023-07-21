using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Avalonia.Media;
using Shiba.Models;
using Shiba.Views;

namespace Shiba.Magic;

public class Conf
{
    public static ConfModel Profile { get; set; } = new();
    public static bool Modify { get; set; }
    public static string SourceDir { get; set; }
    public static string SourceFile { get; set; }
    public const string ConfFile = "conf.json";

    public static MainWindow Win { get; set; }

    public static void FirstLaunch()
    {
        if (File.Exists(ConfFile))
        {
            LoadFile();
        }
        else
        {
            Win.HeadLabel.Content = "No profile found";
        }
    }

    public static void SaveFile(ConfModel conf)
    {
        try
        {
            var options = new JsonSerializerOptions {WriteIndented = true};
            string json = JsonSerializer.Serialize(conf, options);
            File.WriteAllText(ConfFile, json);
        }
        catch (Exception e)
        {
            Error.Warning(e.Message);
            Error.Log(e.ToString());
        }
    }

    public static void LoadFile()
    {
        try
        {
            string json = File.ReadAllText(ConfFile);
            Profile = JsonSerializer.Deserialize<ConfModel>(json);
            SourceDir = Profile.Path;
            SourceFile = Profile.File;
            Error.Log(json);
            Probe.Start();
        }
        catch (Exception e)
        {
            Error.Warning(e.Message);
            Error.Log(e.ToString());
        }
    }
}