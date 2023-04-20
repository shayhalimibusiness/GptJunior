namespace GptJunior;

public interface IProjectManager
{
    string Run();
    IClassManager GetClass(string className);
    IProgramManager GetProgram();
}

public delegate IClassManager CreateClassManager(string className);

public class ProjectManager : IProjectManager
{
    private const string SectionLine = "\n*******************************************\n";
    private const string BuildTitle = "Build:\n";
    private const string RunTitle = "Run:\n";

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

    public string Run()
    {
        _runProjectScriptRunner.Run();

        var feedback =
            SectionLine +
            BuildTitle +
            _feedbackViewer.GetBuildLog() +
            SectionLine +
            RunTitle +
            _feedbackViewer.GetRunLog() +
            SectionLine;

        return feedback;
    }

    public IClassManager GetClass(string className)
    {
        if (_classManagers.TryGetValue(className, out var classManager))
            return classManager;

        var classMgr = _createClassManager(className);
        _classManagers[className] = classMgr;

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