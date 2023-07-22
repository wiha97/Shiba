using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
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

    IBrush[] colors = {Brush.Parse("#252525"), Brush.Parse("Transparent")};

    void FillKeywords()
    {
        keyStack.Children.Clear();
        int idx = 0;
        int i = 0;
        foreach (var word in keywords)
        {
            if (i > 1)
                i = 0;
            UniformGrid uGrid = new()
            {
                Columns = 5,
                Rows = 1
            };
            TextBox key = new()
            {
                Text = word.Name,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                Margin = new Thickness(10, 0, 10, 0)
            };
            key.LostFocus += (sender, args) => word.Name = key.Text;
            TextBox title = new()
            {
                Text = word.Title,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                Margin = new Thickness(10, 0, 10, 0)
            };
            title.LostFocus += (sender, args) => word.Title = title.Text;
            string space = "space";
            if (word.NoSpace == true)
                space = "no space";
            Label spc = new()
            {
                Content = space,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                // Margin = new Thickness(0, 0, 50, 0)
            };
            Button btn = new()
            {
                Background = Brushes.Transparent,
                HorizontalAlignment = HorizontalAlignment.Right,
                Content = "X",
                BorderBrush = Brushes.DarkRed,
                VerticalAlignment = VerticalAlignment.Center
            };
            btn.Click += (sender, args) =>
            {
                keywords.Remove(word);
                FillKeywords();
            };
            DockPanel dock = new()
            {
                Background = colors[i]
            };

            uGrid.Children.Add(key);
            uGrid.Children.Add(title);
            // grid.Children.Add(exp);

            // if (word.Remove.Count > 0)
                uGrid.Children.Add(Filter(word));
            // else
            //     uGrid.Children.Add(new Label(){Content = ""});

            uGrid.Children.Add(spc);
            uGrid.Children.Add(btn);
            dock.Children.Add(uGrid);

            keyStack.Children.Add(dock);
            i++;
            idx++;
        }
    }

    private string FilterBtnText(KeywordModel word)
    {
        string text = "Filter";
        if (word.Remove.Count > 0)
            text += $" ({word.Remove.Count})";
        return text;
    }
    
    private Button Filter(KeywordModel word)
    {
        StackPanel eStack = new();
        Button btn = new()
        {
            Content = FilterBtnText(word),
            Padding = new Thickness(0),
            Background = Brush.Parse("#232323"),
            HorizontalContentAlignment = HorizontalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        Grid aGrid = new();
        TextBox box = new()
        {
            HorizontalAlignment = HorizontalAlignment.Left
        };
        Button aBtn = new()
        {
            Content = "+",
            Padding = new Thickness(0),
            Background = Brushes.Transparent,
            HorizontalAlignment = HorizontalAlignment.Right,
        };
        aBtn.Click += (sender, args) =>
        {
            word.Remove.Add(box.Text);
            FillKeywords();
        };
        aGrid.Children.Add(box);
        aGrid.Children.Add(aBtn);
        eStack.Children.Add(aGrid);
        
        int x = 0;
        foreach (string rem in word.Remove)
        {
            if (x > 1)
                x = 0;
            Grid g = new Grid() {Background = colors[x]};
            Label l = new()
            {
                Content = rem,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 20, 0)
            };
            Button b = new()
            {
            Padding = new Thickness(0),
                Background = Brushes.Transparent,
                Content = "X",
                BorderBrush = Brushes.DarkRed,
                Width = 15,
                Height = 15,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(20, 0, 0, 0)
            };
            b.Click += (sender, args) =>
            {
                word.Remove.Remove(rem);
                FillKeywords();
            };
            g.Children.Add(l);
            g.Children.Add(b);
            eStack.Children.Add(g);
        }

        btn.Flyout = FilterFlyout(word);

        // btn.Content = eStack;
        return btn;
    }

    private Flyout FilterFlyout(KeywordModel word)
    {
        Flyout flyout = new Flyout()
        {
            Placement = FlyoutPlacementMode.Bottom
        };
        StackPanel eStack = new(){MinWidth = 100};
        Grid aGrid = new();
        TextBox box = new()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0,0,25,0)
        };
        Button aBtn = new()
        {
            Content = "+",
            Background = Brushes.Transparent,
            HorizontalAlignment = HorizontalAlignment.Right,
        };
        aBtn.Click += (sender, args) =>
        {
            word.Remove.Add(box.Text);
            FillKeywords();
        };
        aGrid.Children.Add(box);
        aGrid.Children.Add(aBtn);
        eStack.Children.Add(aGrid);
        
        int x = 0;
        foreach (string rem in word.Remove)
        {
            if (x > 1)
                x = 0;
            DockPanel g = new(){Background = colors[x]};
            Label l = new()
            {
                Content = rem,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 20, 0)
            };
            Button b = new()
            {
                Background = Brushes.Transparent,
                Content = "X",
                BorderBrush = Brushes.DarkRed,
                Width = 15,
                Height = 15,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(50, 0, 0, 0)
            };
            b.Click += (sender, args) =>
            {
                word.Remove.Remove(rem);
                FillKeywords();
            };
            DockPanel.SetDock(b, Dock.Right);
            g.Children.Add(b);
            g.Children.Add(l);
            eStack.Children.Add(g);
            x++;
        }
        
        flyout.Content = eStack;
        
        return flyout;
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
                Remove = delWord,
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
        if (profBox.Text != null)
            if (profBox.Text.Contains("{") || profBox.Text.EndsWith(".json"))
                this.Find<Button>("ProfBtn").IsEnabled = true;
    }

    private void SaveBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Conf.Win.Stacky.Children.Clear();
        CreateConf();
        Close();
    }
}