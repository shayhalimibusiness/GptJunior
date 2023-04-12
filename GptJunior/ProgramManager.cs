using System.Text;

namespace GptJunior;

public interface IProgramManager
{
    void AddFlow(string flow);
    void Save();
    void Clean();
}

public class ProgramManager : IProgramManager
{
    private const int FlowBaseLine = 4;
    
    private readonly string _namespaceName;
    private IFileEditor _fileEditor;

    private string _programCode;
    private int _codeLinesNum;

    public ProgramManager(string namespaceName, IFileEditor fileEditor)
    {
        _namespaceName = namespaceName;
        _fileEditor = fileEditor;

        _programCode = GetProgramBaseCode();
        _codeLinesNum = 0;
    }

    public void AddFlow(string flow)
    {
        var lines = flow.Split("\n").ToList();
        _fileEditor.WriteLines(FlowBaseLine + _codeLinesNum, lines);
    }

    public void Save()
    {
        _fileEditor.WriteFile(_programCode);
    }

    public void Clean()
    {
        _fileEditor.CleanFile();
        _programCode = GetProgramBaseCode();
        _codeLinesNum = 0;
    }

    private string GetProgramBaseCode()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"using {_namespaceName};");
        sb.AppendLine("");
        sb.AppendLine("");
        sb.AppendLine("// Add your flow  here");
        sb.AppendLine("");
        sb.AppendLine("");

        return sb.ToString();
    }
}

public static class ProgramManagersFactory
{
    public static IProgramManager CreateProgramManagersFactory()
    {
        const string namespaceName = "PlayGround";
        var fileEditor = FileEditorsFactory.CreateProgramEditor();
        
        var programManager = new ProgramManager(namespaceName, fileEditor);

        return programManager;
    }
}