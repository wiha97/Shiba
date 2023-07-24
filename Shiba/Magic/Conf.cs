using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia.Media;
using Shiba.Models;
using Shiba.Views;

namespace Shiba.Magic;

public class Conf
{
    public static List<string> Profiles { get; set; }
    public static ConfModel Profile { get; set; } = new();
    public static bool Modify { get; set; }
    public static string SourceDir { get; set; }
    public static string SourceFile { get; set; }
    public const string ConfPath = "profiles";
    
    private static string selected = "";

    public static MainWindow Win { get; set; }

    public static void FirstLaunch()
    {
        if (Directory.Exists(ConfPath) && Directory.GetFiles(ConfPath).Length > 0)
        {
            ListProfiles();
            LoadFile(selected);
            // Win.ProfileBox.Items = Profiles;
            // Win.ProfileBox.SelectedIndex = 0;
        }
        else
        {
            Error.Warning("No profile found!");
        }
    }

    public static void SaveFile(ConfModel conf)
    {
        try
        {
            FileManager.DirCheck(ConfPath);
            var options = new JsonSerializerOptions {WriteIndented = true};
            string json = JsonSerializer.Serialize(conf, options);
            File.WriteAllText($"{ConfPath}/{conf.ProfileName}.json", json);
            ListProfiles();
        }
        catch (Exception e)
        {
            Error.Warning($"SF: {e.Message}");
            Error.Log(e.ToString());
        }
    }

    public static void ListProfiles()
    {
        Profiles = Directory.GetFiles(ConfPath).ToList();
        selected = Profiles.OrderByDescending(p => File.GetLastAccessTime(p)).FirstOrDefault();
        Win.ProfileBox.Items = Profiles;
        Win.ProfileBox.SelectedItem = selected;
    }

    public static void LoadFile(string profile)
    {
        try
        {
            string json = File.ReadAllText($"{profile}");
            Profile = JsonSerializer.Deserialize<ConfModel>(json);
            SourceDir = Profile.Path;
            SourceFile = Profile.File;
            Probe.Start();
        }
        catch (Exception e)
        {
            Error.Warning(Profiles[0]);
            Error.Log(e.ToString());
        }
    }
}