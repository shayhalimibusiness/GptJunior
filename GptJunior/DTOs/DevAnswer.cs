namespace GptJunior.Modules;

public class DevAnswer
{
    public string? Inter { get; set; }
    public List<CodeFile>? Implementation { get; set; }
    public List<CodeFile>? Tests { get; set; }
}

public class CodeFile
{
    public string? Name { get; set; }
    public string? Code { get; set; }
}