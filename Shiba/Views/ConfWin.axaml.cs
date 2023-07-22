using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Shiba.Magic;
using Shiba.Models;

namespace Shiba.Views;

public partial class ConfWin : Window
{
    private TextBox profBox;
    private TextBox pathBox;
    private TextBox fileBox;
    private TextBox stringBox;
    private TextBox sepBox;
    private TextBox keyBox;
    private TextBox keyTitleBox;
    private TextBox delBox;
    private CheckBox spaceBox;
    private StackPanel delStack;
    private StackPanel keyStack;

    private ConfModel conf;

    private List<string> delWord = new();
    private List<KeywordModel> keywords = new();

    public ConfWin()
    {
        conf = Conf.Profile;
        InitializeComponent();
        profBox = this.Find<TextBox>("ProfBox");
        pathBox = this.Find<TextBox>("PathBox");
        fileBox = this.Find<TextBox>("FileBox");
        stringBox = this.Find<TextBox>("FullStringBox");
        sepBox = this.Find<TextBox>("SepBox");
        keyBox = this.Find<TextBox>("KeyBox");
        keyTitleBox = this.Find<TextBox>("KeyTitleBox");
        delBox = this.Find<TextBox>("DelBox");
        spaceBox = this.Find<CheckBox>("SpaceBox");
        delStack = this.Find<StackPanel>("DelStack");
        keyStack = this.Find<StackPanel>("KeyStack");

        if (Conf.Modify)
            FillConf();

        sepBox.Text = conf.Separator.ToString();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    void FillConf()
    {
        this.Find<Button>("JsonBtn").IsEnabled = true;
        profBox.Text = conf.ProfileName;
        pathBox.Text = conf.Path;
        fileBox.Text = conf.File;
        stringBox.Text = conf.FullString;
        keywords = conf.Keywords;
        FillKeywords();
    }

    void CreateConf()
    {
        try
        {
            conf = new ConfModel()
            {
                ProfileName = profBox.Text,
                Path = pathBox.Text,
                File = fileBox.Text,
                FullString = stringBox.Text,
                Separator = sepBox.Text[0],
                Keywords = keywords
            };
            if (conf.Path != null && !conf.Path.EndsWith("/"))
                conf.Path += "/";
            Conf.SaveFile(conf);
            Conf.Win.ProfileBtn.Content = "Modify Profile";
            Conf.Modify = true;
            Conf.LoadFile();
        }
        catch (Exception e)
        {
            Error.Warning(e.Message);
            Error.Log(e.ToString());
        }
    }

    private void SaveBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        CreateConf();
        Close();
    }

    private void AddDelBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        delWord.Add(delBox.Text);
        delStack.Children.Clear();
        foreach (string word in delWord)
        {
            Label label = new()
            {
                Foreground = Brushes.Azure,
                Content = word
            };
            delBox.Text = "";
            delStack.Children.Add(label);
        }
    }

    void FillKeywords()
    {
        keyStack.Children.Clear();
        int i = 0;
        IBrush[] colors = {Brush.Parse("#252525"), Brush.Parse("Transparent")};
        foreach (var word in keywords)
        {
            if (i > 1)
                i = 0;
            Grid grid = new();
            Label key = new()
            {
                Foreground = Brushes.Azure,
                Content = word.Name,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10, 0, 10, 0)
            };
            ListBox list = new()
            {
                Items = word.Remove,
                Foreground = Brushes.Azure
            };
            Expander exp = new()
            {
                Header = word.Remove.Length,
                Content = list,
                Padding = new Thickness(0),
                Background = Brushes.Transparent,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            RadioButton rad = new()
            {
                IsEnabled = false,
                IsChecked = word.NoSpace,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            DockPanel dock = new()
            {
                Background = colors[i]
            };
            grid.Children.Add(key);
            grid.Children.Add(exp);
            grid.Children.Add(rad);
            dock.Children.Add(grid);

            keyStack.Children.Add(dock);
            i++;
        }
    }

    private string Json(string json)
    {
        if (json.EndsWith(".json"))
        {
            json = File.ReadAllText(json);
        }

        return json;
    }

    private void ImportProfile(string json)
    {
        try
        {
            conf = JsonSerializer.Deserialize<ConfModel>(json);
            FillConf();
        }
        catch (Exception e)
        {
            Error.Warning(e.Message);
            Error.Log(e.ToString());
        }
    }

    private void ImportKeywords(string json)
    {
        try
        {
            List<KeywordModel> imported = JsonSerializer.Deserialize<List<KeywordModel>>(json);
            foreach (KeywordModel key in imported)
            {
                keywords.Add(key);
            }
        }
        catch (Exception e)
        {
            Error.Warning(e.Message);
            Error.Log(e.ToString());
        }
    }

    private void AddKeyBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        delStack.Children.Clear();
        if (keyBox.Text.Contains("{") || keyBox.Text.EndsWith(".json"))
        {
            ImportKeywords(Json(keyBox.Text));
        }
        else
        {
            keywords.Add(new()
            {
                Name = keyBox.Text,
                Title = keyTitleBox.Text,
                Remove = delWord.ToArray(),
                NoSpace = spaceBox.IsChecked
            });
            keyTitleBox.Clear();
            spaceBox.IsChecked = false;
            delWord.Clear();
        }

        FillKeywords();
    }

    private void JsonBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start(Conf.ConfFile);
        }
        catch (Exception x)
        {
            Error.Warning(x.Message);
            Error.Log(x.ToString());
        }
    }

    private void ProfBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        ImportProfile(Json(profBox.Text));
    }

    private void ProfBox_OnGotFocus(object? sender, GotFocusEventArgs e)
    {
        if(profBox.Text != null) 
            if(profBox.Text.Contains("{")||profBox.Text.EndsWith(".json")) 
                this.Find<Button>("ProfBtn").IsEnabled = true;
    }
}