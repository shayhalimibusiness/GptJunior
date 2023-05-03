namespace GptJunior.Modules;

public class DevAnswer
{
    public string? Name { get; set; }
    public string? Inter { get; set; }
    public List<FileDto>? Implementation { get; set; }
    public List<FileDto>? Tests { get; set; }
}