using System;
using System.IO;

namespace Shiba.Magic;

public class FileManager
{
    
    public static void MoveFile(string file)
    {
        try
        {
            string target = $"{Conf.SourceDir}/processed";
            DirCheck(target);
            string time = DateTime.Now.ToString("MMddHHmm");
            File.Move(file, $"{target}/{Conf.SourceFile}_{time}.txt");
        }
        catch (Exception e)
        {
            Error.Warning(e.Message);
        }
    }
    
    public static void DirCheck(string dir)
    {
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }
}