using GptJunior.Modules;

namespace GptJunior.IDE;

public interface IIde
{
    string Read(string file);
    void Write(string file, string content);
    dynamic Run();
}

public class Ide : IIde
{
    private readonly string _rootDirectoryPath;
    private readonly IFeedbackViewer _feedbackViewer;
    private readonly IScriptRunner _scriptRunner;
    
    public Ide(string rootDirectoryPath, IFeedbackViewer feedbackViewer, IScriptRunner scriptRunner)
    {
        _rootDirectoryPath = rootDirectoryPath;
        _feedbackViewer = feedbackViewer;
        _scriptRunner = scriptRunner;
    }
    
    public string Read(string file)
    {
        var fullPath = Path.Join(_rootDirectoryPath, file);
        IFileEditor fileEditor = new FileEditor(fullPath);
        return fileEditor.ReadFile();
    }

    public void Write(string file, string content)
    {
        var fullPath = Path.Join(_rootDirectoryPath, file);
        IFileEditor fileEditor = new FileEditor(fullPath + ".cs");
        fileEditor.WriteFile(content);
    }

    public dynamic Run()
    {
        _scriptRunner.Run();
        
        var runDto = new RunDto
        {
            Build = _feedbackViewer.GetBuildLog(),
            Run = _feedbackViewer.GetRunLog()
        };
        
        return runDto;
    }
}

public static class IdesFactory
{
    public static IIde CreateIde()
    {
        const string rootDirectoryPath = @"C:\Users\shay.halimi\Desktop\PrivateWorkPlace\PlayGround\PlayGround";
        var feedbackViewer = FeedbackViewersFactory.CreateProjectFeedbackViewer();
        var scriptRunner = ScriptRunnerFactory.CreateRunProjectScriptRunner();
        
        var ide = new Ide(rootDirectoryPath, feedbackViewer, scriptRunner);

        return ide;
    }
}