namespace GptJunior.Modules;

public class FixRequest
{
    public string? Description { get; set; }
    public string? Build { get; set; }
    public string? Run { get; set; }
    public List<FileDto>? Files { get; set; }
}