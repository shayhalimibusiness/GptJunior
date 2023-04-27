namespace GptJunior;

public interface IProjectManager
{
    dynamic Run();
    IClassManager GetClass(string className);
    IProgramManager GetProgram();
}

public delegate IClassManager CreateClassManager(string className);

public class ProjectManager : IProjectManager
{
    private readonly IProgramManager _programManager;
    private readonly CreateClassManager _createClassManager;
    private readonly IScriptRunner _runProjectScriptRunner;
    private readonly IFeedbackViewer _feedbackViewer;
    
    private readonly Dictionary<string, IClassManager> _classManagers;
    
    public ProjectManager(
        IProgramManager programManager, 
        CreateClassManager createClassManager, 
        IScriptRunner runProjectScriptRunner, 
        IFeedbackViewer feedbackViewer)
    {
        _programManager = programManager;
        _createClassManager = createClassManager;
        _runProjectScriptRunner = runProjectScriptRunner;
        _feedbackViewer = feedbackViewer;

        _classManagers = new Dictionary<string, IClassManager>();
    }

    public dynamic Run()
    {
        _runProjectScriptRunner.Run();

        // Todo: this works only if there is a single class need to work for multiple classes.
        var classCs = _classManagers.ToList()[0].Value.Read();
        var functionName = _classManagers.ToList()[0].Key.Split("Class")[0];
        var programCs = _programManager.Read();

        var result = new
        {
            FunctionName = functionName,
            ClassCs = classCs,
            ProgramCs = programCs,
            BuildLog = _feedbackViewer.GetBuildLog(),
            RunLog = _feedbackViewer.GetRunLog()
        };
        return result;
    }

    public IClassManager GetClass(string className)
    {
        const string classNamePostfix = "Class";
        
        if (_classManagers.TryGetValue(className, out var classManager))
            return classManager;

        var classMgr = _createClassManager(className + classNamePostfix);
        _classManagers[className] = classMgr;
        classMgr.CreateFile();

        return classMgr;
    }

    public IProgramManager GetProgram()
    {
        return _programManager;
    }
}

public static class ProjectManagersFactory
{
    public static IProjectManager CreateProjectManager()
    {
        var programManager = ProgramManagersFactory.CreateProgramManagersFactory();
        CreateClassManager createClassManager = ClassManagersFactory.CreateClassManager;
        var runProjectScriptRunner = ScriptRunnerFactory.CreateRunProjectScriptRunner();
        var feedbackViewer = FeedbackViewersFactory.CreateProjectFeedbackViewer();
        
        var projectManager = new ProjectManager(
            programManager,
            createClassManager,
            runProjectScriptRunner,
            feedbackViewer);

        return projectManager;
    }
}