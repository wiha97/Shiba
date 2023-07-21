using System.Collections.Generic;

namespace Shiba.Models;

public class KeywordModel
{
    public string Name { get; set; }
    public string? Title { get; set; }
    public string? Value { get; set; }
    public string[] Remove { get; set; }
    
    public bool? NoSpace { get; set; }
    
}