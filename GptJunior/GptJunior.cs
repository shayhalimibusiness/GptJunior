namespace GptJunior;

public interface IGptJunior
{
    Task Create(string description);
}

public class GptJunior : IGptJunior
{
    private readonly IProjectManager _projectManager;
    private readonly IGptDeveloper _gptDeveloper;
    private readonly IGitManager _gitManager;

    public GptJunior(
        IProjectManager projectManager, 
        IGptDeveloper gptDeveloper, 
        IGitManager gitManager)
    {
        _projectManager = projectManager;
        _gptDeveloper = gptDeveloper;
        _gitManager = gitManager;
    }

    public async Task Create(string description)
    {
        var functionName = await Develop(description);
        await Fix(functionName);
    }

    private async Task<string> Develop(string description)
    {
        var programManager = _projectManager.GetProgram();
        
        var response = await _gptDeveloper.Develop(description);
        
        string functionName = response.FunctionName[0];
        List<string> functionImplementation = response.FunctionImplementation;
        List<string> flow = response.Flow;
        var expectedResult = response.ExpectedResult;

        var classManager = _projectManager.GetClass(functionName);
        classManager.AddFunction(functionImplementation);
        var testFunction = Helpers.CreateFunctionWrapper("Test");
        flow = flow.Select(line => line = "   " + line).ToList();
        testFunction.InsertRange(2, flow);
        classManager.AddFunction(testFunction);
        
        programManager.AddFlow(new List<string> {$"var test = new {functionName}Class();"});
        programManager.Save();

        return functionName;
    }

    private async Task Fix(string functionName)
    {
        // Todo: Remember past iterations
        for (var i = 0; i < 10; i++)
        {
            var feedback = _projectManager.Run();
            var response = await _gptDeveloper.Fix(feedback);
            string result = response.Result[0];
            string newProgramCs = string.Join('\n', response.ProgramCs);;
            string newClassCs = string.Join('\n', response.ClassCs);
            switch (result)
            {
                case "PASS!":
                    return;
                case "FAILED!":
                    var classEditor = FileEditorsFactory.CreateClassEditor(functionName + "Class");
                    classEditor.WriteFile(newClassCs);
                    var programEditor = FileEditorsFactory.CreateProgramEditor();
                    programEditor.WriteFile(newProgramCs);
                    break;
                default:
                    throw new Exception("Function returned invalid response");
            }
        }
    }
}

public static class GptJuniorsFactory
{
    public static IGptJunior CreateGptJunior()
    {
        var projectManager = ProjectManagersFactory.CreateProjectManager();
        var gptDeveloper = GptDevelopersFactory.CreateGptDeveloper();
        var gitManager = GitManagersFactory.CreateGitManager();

        var gptJunior = new GptJunior(
            projectManager, 
            gptDeveloper,
            gitManager);

        return gptJunior;
    }
}