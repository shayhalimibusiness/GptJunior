namespace GptJunior;

public interface IGptDeveloper
{
    Task<dynamic> Develop(string request);
    Task<dynamic> Fix(string feedback);
}

public class ProjectDeveloper : IGptDeveloper
{
    private readonly IGptProxy _developer;
    private readonly IGptProxy _fixer;
    private readonly IGptAdaptor _translator;

    public ProjectDeveloper(IGptProxy developer, IGptProxy fixer, IGptAdaptor translator)
    {
        _developer = developer;
        _fixer = fixer;
        _translator = translator;
    }

    public async Task<dynamic> Develop(string request)
    {
        var textResponse = await _developer.GetResponse(request);
        var sections = _translator.Translate(textResponse).ToArray();
        var functionName = sections[0];
        var functionImplementation = sections[1];
        var flow = sections[2];
        var expectedResult = sections[3];
        var structuredResponse = new
        {
            FunctionName = functionName, 
            FunctionImplementation = functionImplementation, 
            Flow = flow,
            ExpectedResult = expectedResult,
        };
        return structuredResponse;
    }

    public async Task<dynamic> Fix(string feedback)
    {
        var textResponse = await _fixer.GetResponse(feedback);
        var sections = _translator.Translate(textResponse).ToArray();
        var result = sections[0];
        var programCs = sections[1];
        var classCs = sections[2];
        var structuredResponse = new
        {
            Result = result,
            ProgramCs = programCs, 
            ClassCs = classCs,
        };
        return structuredResponse;
    }
}

public static class GptDevelopersFactory
{
    public static IGptDeveloper CreateGptDeveloper()
    {
        var developer = GptProxiesFactory.CreateGptFunctionDeveloper();
        var fixer = GptProxiesFactory.CreateGptFunctionFixer();
        var translator = GptAdaptorsFactory.CreateGptAdaptor();
     
        var gptDeveloper = new ProjectDeveloper(developer, fixer, translator);

        return gptDeveloper;
    }
}

