using GptJunior.Modules;

namespace GptJunior;

public interface IGptDeveloper
{
    Task<dynamic> Develop(string request);
    Task<dynamic> Fix(dynamic feedback);
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
        List<List<string>> sections = _translator.Translate(textResponse).ToArray();
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

    public async Task<dynamic> Fix(dynamic feedback)
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

public class JuniorDeveloper : IGptDeveloper
{
    private readonly IAiAdaptor _developerAdaptor;
    private readonly IAiAdaptor _fixerAdaptor;

    public JuniorDeveloper(
        IAiAdaptor developerAdaptor, 
        IAiAdaptor fixerAdaptor)
    {
        _developerAdaptor = developerAdaptor;
        _fixerAdaptor = fixerAdaptor;
    }

    public async Task<dynamic> Develop(string request)
    {
        DevAnswer? development = await _developerAdaptor.Respond(request);
        if (development == null)
        {
            throw new Exception("Interface developer returned null");
        }

        return development;
    }

    public async Task<dynamic> Fix(dynamic feedback)
    {
        FixAnswer? fix = await _fixerAdaptor.Respond(feedback);
        if (fix == null)
        {
            throw new Exception("Interface fixer returned null");
        }

        return fix;
    }
}

public static class GptDevelopersFactory
{
    public static IGptDeveloper CreateBaseGptDeveloper()
    {
        var developer = GptProxiesFactory.CreateGptFunctionDeveloper();
        var fixer = GptProxiesFactory.CreateGptFunctionFixer();
        var translator = GptAdaptorsFactory.CreateGptAdaptor();
     
        var gptDeveloper = new ProjectDeveloper(developer, fixer, translator);

        return gptDeveloper;
    }
    
    public static IGptDeveloper CreateJuniorDeveloper()
    {
        var developerAdaptor = AiAdaptorsFactory.CreateInterfaceDeveloperAdaptor();
        var fixerAdaptor = AiAdaptorsFactory.CreateInterfaceFixerAdaptor();
     
        var gptDeveloper = new JuniorDeveloper(developerAdaptor, fixerAdaptor);

        return gptDeveloper;
    }
}

