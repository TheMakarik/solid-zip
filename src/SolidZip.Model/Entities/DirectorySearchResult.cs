namespace SolidZip.Model.Entities;

public struct DirectorySearchResult(string beforePattern, string pattern, string afterPattern)
{
    public string BeforePattern { get; set; } = beforePattern;
    public string Pattern { get; set; } = pattern;
    public string AfterPattern { get; set; } = afterPattern;

    public readonly bool IsEmpty 
        => string.IsNullOrEmpty(BeforePattern) && 
           string.IsNullOrEmpty(Pattern) && 
           string.IsNullOrEmpty(AfterPattern);
}