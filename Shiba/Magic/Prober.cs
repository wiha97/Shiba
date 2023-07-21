using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Shiba.Models;

namespace Shiba.Magic;

public class Probe
{
    public static void Start()
    {
        Filter();
    }

    private static ConfModel conf = new();


    public static void Filter()
    {
        try
        {
            string json = File.ReadAllText(Conf.ConfFile);
            conf = JsonSerializer.Deserialize<ConfModel>(json);
            List<Grid> grids = new();
            string[] file = File.ReadAllLines($"{conf.Path}{conf.File}");
            int i = 0;
            foreach (string line in file)
            {
                foreach (KeywordModel keyword in conf.Keywords)
                {
                    if (line.StartsWith(keyword.Name))
                    {
                        if (i > 1)
                            i = 0;
                        keyword.Value = line.Substring(line.IndexOf(conf.Separator) + 1);
                        keyword.Value = RuleCheck(keyword);
                        grids.Add(Row(keyword, i));
                        conf.FullString += $",{keyword.Value}";
                        i++;
                        break;
                    }
                }
            }

            foreach (Grid grid in grids)
            {
                Conf.Win.Stacky.Children.Add(grid);
            }

            Conf.Win.FullString.Text = conf.FullString;
        }
        catch (FileNotFoundException e)
        {
            Error.Warning(e.Message);
        }
        catch (Exception e)
        {
            Error.Warning(e.Message);
            Error.Log(e.ToString());
        }
    }

    static Grid Row(KeywordModel keyword, int i)
    {
        IBrush[] colors = new[] {Brush.Parse("#252525"), Brush.Parse("Transparent")};
        Grid grid = new() {Background = colors[i]};
        try
        {
            Label lab = new()
            {
                Foreground = Brushes.Azure,
                HorizontalAlignment = HorizontalAlignment.Left,
                Content = keyword.Title
            };
            TextBox box = new()
            {
                IsReadOnly = true,
                Foreground = Brushes.Azure,
                Margin = new Thickness(125, 0, 0, 0),
                HorizontalContentAlignment = HorizontalAlignment.Right,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                Text = keyword.Value
            };

            grid.Children.Add(lab);
            grid.Children.Add(box);
        }
        catch (Exception e)
        {
            Error.Warning(e.Message);
        }

        return grid;
    }

    static string RuleCheck(KeywordModel keyword)
    {
        string value = keyword.Value;
        
        if (keyword.Remove.Length > 0)
        {
            value = RemoveWord(value, keyword.Remove);
        }
        if (keyword.NoSpace == true)
        {
            value = NoSpace(value);
        }

        return value;
    }

    static string NoSpace(string value)
    {
        return value.Replace(" ", "");
    }

    static string RemoveWord(string value, string[] words)
    {
        foreach (string word in words)
        {
            value = value.Replace(word, "");
        }

        return value;
    }
}