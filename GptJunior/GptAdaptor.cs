namespace GptJunior;

public interface IGptAdaptor
{
    IEnumerable<IEnumerable<string>> Translate(string response);
}

public class GptAdaptor : IGptAdaptor
{
    public IEnumerable<IEnumerable<string>> Translate(string response)
    {
        var lines = response.Split("\n");
        var section = new List<string>();
        var sections = new List<List<string>>();
        
        
        foreach (var line in lines)
        {
            if (line.Contains("---"))
            {
                sections.Add(section);
                section = new List<string>();
                continue;
            }
            
            section.Add(line);
        }

        return sections;
    }
}

public static class GptAdaptorsFactory
{
    public static IGptAdaptor CreateGptAdaptor()
    {
        return new GptAdaptor();
    }
}