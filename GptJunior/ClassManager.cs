using System.Text;

namespace GptJunior;

public interface IClassManager
{
    void CreateFile();
    void AddMember(string member);
    void AddFunction(string function);
    void Clean();
}

public class ClassManager : IClassManager
{
    private const int MemBaseLine = 5;
    private const int FuncBaseLine = 7;
    private const string Indentation = "        ";

    private string _className;
    private string _namespaceName;
    private IFileEditor _fileEditor;

    private string _classCode;
    private int _memNum;
    private int _funcLines;

    public ClassManager(string className, string namespaceName, IFileEditor fileEditor)
    {
        _className = className;
        _namespaceName = namespaceName;
        _fileEditor = fileEditor;

        _funcLines = 0;
        _memNum = 0;

        _classCode = GetClassBaseCode();
    }


    public void CreateFile()
    {
        _fileEditor.WriteFile(_classCode);
    }

    public void AddMember(string member)
    {
        var lines = new List<string>{Indentation + member};
        _fileEditor.WriteLines(MemBaseLine + 2 * _memNum, lines);
        _memNum++;
    }

    public void AddFunction(string function)
    {
        var lines = function.Split("\n").ToList();
        lines = lines.Select(line => Indentation + line).ToList();
        _fileEditor.WriteLines(FuncBaseLine + _funcLines, lines);
        _funcLines += lines.Count + 1;
    }

    public void Clean()
    {
        _fileEditor.CleanFile();
        _classCode = GetClassBaseCode();
        _memNum = 0;
        _funcLines = 0;
    }

    private string GetClassBaseCode()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"namespace {_namespaceName}");
        sb.AppendLine("{");
        sb.AppendLine($"    public class {_className}");
        sb.AppendLine("    {");
        sb.AppendLine("        // Add your class members  here");
        sb.AppendLine("        ");
        sb.AppendLine("        // Add your class  methods here");
        sb.AppendLine("        ");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }
}

public static class ClassManagersFactory
{
    public static IClassManager CreateClassManager(string className)
    {
        var fileEditor = FileEditorsFactory.CreateClassEditor(className);
        const string namespaceName = "PlayGround";

        var classManager = new ClassManager(className, namespaceName, fileEditor);

        return classManager;
    }
}