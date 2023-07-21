using System;
using System.IO;

namespace Shiba.Magic;

public class Error
{
    public static void Log(string msg)
    {
        FileManager.DirCheck("errors");
        string file = $"errors/error-{DateTime.Now.ToString("HH-mm-ss_dd-MM-yy")}.log";
        File.WriteAllText(file, msg);
    }

    public static void Warning(string msg)
    {
        Conf.Win.HeadLabel.Content = msg;
    }
}