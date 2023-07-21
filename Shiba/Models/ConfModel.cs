using System.Collections.Generic;

namespace Shiba.Models;

public class ConfModel
{
    public string Path { get; set; } = "null";
    public string File { get; set; }
    public string FullString { get; set; }
    public char Separator { get; set; } = ':';
    public List<KeywordModel> Keywords { get; set; }
}