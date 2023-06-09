namespace GptJunior;

public interface IFileEditor
{
    void SetFile(string path);
    
    string ReadFile();

    List<string> ReadLines(int start, int end);

    void WriteFile(string text);

    void WriteLines(int start, List<string> text);

    void DeleteLines(int start, int num);

    void AppendText(string text);

    void CleanFile();
}

public class FileEditor : IFileEditor
{
    private string _filePath;

    public FileEditor(string filePath)
    {
        _filePath = filePath;
    }

    public void SetFile(string path)
    {
        _filePath = path;
    }

    public string ReadFile()
    {
        using var fileStream = new FileStream(
            _filePath, 
            FileMode.OpenOrCreate, 
            FileAccess.Read, 
            FileShare.ReadWrite);
        
        using var streamReader = new StreamReader(fileStream);
        
        return streamReader.ReadToEnd();
    }

    public List<string> ReadLines(int start, int end)
    {
        List<string> lines = new List<string>();

        using (FileStream fileStream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
        using (StreamReader streamReader = new StreamReader(fileStream))
        {
            int lineNumber = 0;
            string line;

            while ((line = streamReader.ReadLine()) != null)
            {
                lineNumber++;

                if (lineNumber >= start && lineNumber <= end)
                {
                    lines.Add(line);
                }
            }
        }

        return lines;
    }

    public void WriteFile(string content)
    {
        using var fileStream = new FileStream(
            _filePath, 
            FileMode.Create, 
            FileAccess.Write, 
            FileShare.ReadWrite);
        
        using var streamWriter = new StreamWriter(fileStream);
        
        streamWriter.Write(content);
    }

    public void WriteLines(int start, List<string> text)
    {
        List<string> lines;

        using (var readStream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
        using (var streamReader = new StreamReader(readStream))
        {
            lines = new List<string>(streamReader.ReadToEnd().Split('\n'));
        }

        lines.InsertRange(start - 1, text);

        using (var writeStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
        using (var streamWriter = new StreamWriter(writeStream))
        {
            streamWriter.Write(string.Join("\n", lines));
        }
    }

    public void DeleteLines(int start, int num)
    {
        List<string> lines;

        using (FileStream fileStream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
        using (StreamReader streamReader = new StreamReader(fileStream))
        {
            lines = new List<string>(streamReader.ReadToEnd().Split('\n'));
        }

        lines.RemoveRange(start - 1, num);

        using (FileStream fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
        using (StreamWriter streamWriter = new StreamWriter(fileStream))
        {
            streamWriter.Write(string.Join("\n", lines));
        }
    }

    public void AppendText(string content)
    {
        using var fileStream = new FileStream(
            _filePath, 
            FileMode.Append, 
            FileAccess.Write, 
            FileShare.ReadWrite);
        
        using var streamWriter = new StreamWriter(fileStream);
        
        streamWriter.Write(content);
    }

    public void CleanFile()
    {
        WriteFile("");
    }
}


public static class FileEditorsFactory
{
    public static IFileEditor CreateProgramEditor()
    {
        return new FileEditor(Common.PlayGroundDirPath + @"Program.cs");
    }

    public static IFileEditor CreateBuildLogEditor()
    {
        return new FileEditor(Common.OutputDirPath + @"build_log.txt");
    }
    
    public static IFileEditor CreateRunLogEditor()
    {
        return new FileEditor(Common.OutputDirPath + @"run_log.txt");
    }

    public static IFileEditor CreateConversationLogEditor()
    {
        return new FileEditor(Common.OutputDirPath + @"conversation_log.txt");
    }

    public static IFileEditor CreateClassEditor(string className)
    {
        return new FileEditor(Common.PlayGroundDirPath + className + ".cs");
    }
    
    public static IFileEditor CreateCreateBranchEditor()
    {
        return new FileEditor(Common.ScriptsDirPath + "CreateBranch.bat");
    }
    
    public static IFileEditor CreateCommitChangesEditor()
    {
        return new FileEditor(Common.ScriptsDirPath + "CommitChanges.bat");
    }
}