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

    private ProgramManager _programManager;
    private CreateClassManager _createClassManager;
    private IScriptRunner _runProjectScriptRunner;
    private IFeedbackViewer _feedbackViewer;
    
    private Dictionary<string, IClassManager> _classManagers;


    public ProjectManager(
        ProgramManager programManager, 
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