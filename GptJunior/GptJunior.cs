namespace GptJunior;

public interface IGptJunior
{
    Task Create(string description);
}

public class GptJunior : IGptJunior
{
    private readonly IProjectManager _projectManager;
    private readonly IGptDeveloper _gptDeveloper;

    public GptJunior(IProjectManager projectManager, IGptDeveloper gptDeveloper)
    {
        _projectManager = projectManager;
        _gptDeveloper = gptDeveloper;
    }

    public async Task Create(string description)
    {
        var response = await _gptDeveloper.Develop(description);
        
        string functionName = response.FunctionName[0];
        List<string> functionImplementation = response.FunctionImplementation;
        List<string> flow = response.Flow;
        var expectedResult = response.ExpectedResult;

        IClassManager classManager = _projectManager.GetClass(functionName);
        classManager.AddFunction(functionImplementation);
        var testFunction = Helpers.CreateFunctionWrapper("Test");
        flow = flow.Select(line => line = "   " + line).ToList();
        testFunction.InsertRange(2, flow);
        classManager.AddFunction(testFunction);
        
        var programManager = _projectManager.GetProgram(); 
        programManager.AddFlow(new List<string> {$"var test = new {functionName}Class();"});
        programManager.Save();
        
    }
}

public static class GptJuniorsFactory
{
    public static IGptJunior CreateGptJunior()
    {
        var projectManager = ProjectManagersFactory.CreateProjectManager();
        var gptDeveloper = GptDevelopersFactory.CreateGptDeveloper();

        var gptJunior = new GptJunior(projectManager, gptDeveloper);

        return gptJunior;
    }
}