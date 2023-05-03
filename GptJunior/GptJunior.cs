using GptJunior.IDE;
using GptJunior.Modules;

namespace GptJunior;

public interface IGptJunior
{
    Task Create(string description);
}

public class BaseGptJunior : IGptJunior
{
    private readonly IProjectManager _projectManager;
    private readonly IGptDeveloper _gptDeveloper;
    private readonly IGitManager _gitManager;

    public BaseGptJunior(
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
        var result = await Develop(description);
        string expectedResult = result.ExpectedResult;
        await Fix(expectedResult);
        _gitManager.CommitChanges("Initial Commit.");
    }

    private async Task<dynamic> Develop(string description)
    {
        var programManager = _projectManager.GetProgram();
        
        var response = await _gptDeveloper.Develop(description);
        
        string functionName = response.FunctionName[0];
        List<string> functionImplementation = response.FunctionImplementation;
        List<string> flow = response.Flow;
        string expectedResult = response.ExpectedResult[0];

        _gitManager.CreateBranch("project_" + functionName);
        
        var classManager = _projectManager.GetClass(functionName);
        classManager.AddFunction(functionImplementation);
        var testFunction = Helpers.CreateFunctionWrapper("Test");
        flow = flow.Select(line => "   " + line).ToList();
        testFunction.InsertRange(2, flow);
        classManager.AddFunction(testFunction);
        
        programManager.AddFlow(new List<string> {$"var test = new {functionName}Class();"});
        programManager.Save();

        dynamic result = new
        {
            FunctionName = functionName, 
            ExpectedResult = expectedResult
        };
        
        return result;
    }

    private async Task Fix(string expectedResult)
    {
        // Todo: Remember past iterations
        for (var i = 0; i < 10; i++)
        {
            var res = _projectManager.Run();
            string functionName = res.FunctionName;
            string classCs = res.ClassCs;
            string programCs = res.ProgramCs;
            string buildLog = res.BuildLog;
            string runLog = res.RunLog;


            var feedback =
                Common.SectionLine +
                functionName +
                Common.SectionLine +
                Common.ClassTitle +
                classCs +
                Common.SectionLine +
                Common.ProgramTitle +
                programCs +
                Common.SectionLine +
                Common.BuildTitle +
                buildLog +
                Common.SectionLine +
                Common.RunTitle +
                runLog +
                Common.SectionLine +
                Common.ExpectedRunTitle +
                expectedResult +
                Common.SectionLine;
            
            var response = await _gptDeveloper.Fix(feedback);
            string result = response.Result[0];
            string newProgramCs = string.Join('\n', response.ProgramCs);
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

public class GptJunior : IGptJunior
{
    private readonly IGptDeveloper _gptDeveloper;
    private readonly IGitManager _gitManager;
    private readonly IIde _ide;

    public GptJunior(
        IGptDeveloper gptDeveloper, 
        IGitManager gitManager, 
        IIde ide)
    {
        _gptDeveloper = gptDeveloper;
        _gitManager = gitManager;
        _ide = ide;
    }

    public async Task Create(string interfaceDescription)
    {
        DevAnswer devAnswer = await _gptDeveloper.Develop(interfaceDescription);

        _gitManager.CreateBranch("project_" + devAnswer.Name);
        foreach (var fileDto in devAnswer.Implementation)
        {
            _ide.Write(fileDto.FileName, fileDto.FileContent);            
        }
        
        foreach (var codeFile in devAnswer.Tests)
        {
            _ide.Write(codeFile.FileName, codeFile.FileContent);            
        }

        for (var i = 0; i < 3; i++)
        {
            RunDto runDto = _ide.Run();

            var files = devAnswer.Implementation
                .Concat(devAnswer.Tests)
                .ToList();

            var fixRequest = new FixRequest
            {
                Run = runDto.Run,
                Build = runDto.Build,
                Description = interfaceDescription,
                Files = files,
            };
        
            FixAnswer fixAnswer = await _gptDeveloper.Fix(fixRequest);

            if (fixAnswer.Files.Count == 0)
            {
                return;
            }

            foreach (var fileDto in fixAnswer.Files)
            {
                _ide.Write(fileDto.FileName, fileDto.FileContent);
            }
        }

        _gitManager.CommitChanges("Initial Commit.");
    }
}

public static class GptJuniorsFactory
{
    public static IGptJunior CreateBaseGptJunior()
    {
        var projectManager = ProjectManagersFactory.CreateProjectManager();
        var gptDeveloper = GptDevelopersFactory.CreateBaseGptDeveloper();
        var gitManager = GitManagersFactory.CreateGitManager();

        var gptJunior = new BaseGptJunior(
            projectManager, 
            gptDeveloper,
            gitManager);

        return gptJunior;
    }
    
    public static IGptJunior CreateGptJunior()
    {
        var gptDeveloper = GptDevelopersFactory.CreateBaseGptDeveloper();
        var gitManager = GitManagersFactory.CreateGitManager();
        var ide = IdesFactory.CreateIde();

        var gptJunior = new GptJunior(
            gptDeveloper,
            gitManager,
            ide);

        return gptJunior;
    }
}