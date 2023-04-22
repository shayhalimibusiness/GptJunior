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
            if (line.Contains(">>>"))
            {
                sections.Add(section);
                section = new List<string>();
                continue;
            }

            if (line.Contains("PASS!"))
            {
                section = new List<string> { "PASS!" };
                sections.Add(section);
                sections.Add(new List<string>());
                sections.Add(new List<string>());
                return sections;
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