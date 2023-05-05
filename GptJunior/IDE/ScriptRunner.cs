using System.Diagnostics;

namespace GptJunior;

public interface IScriptRunner
{
    string Run();
}

public class ScriptRunner : IScriptRunner
{
    public ScriptRunner(string path)
    {
        Path = path;
    }

    public string Path { get; set; }
    
    public string Run()
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = $"/C \"{Path}\""
                }
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error executing batch script: " + ex.Message);
            return "Error!!!";
        }
    }
}

public static class ScriptRunnerFactory
{
    public static IScriptRunner CreateRunProjectScriptRunner()
    {
        const string scriptName = "RunProject";
        return new ScriptRunner(Common.ScriptsDirPath + scriptName + Common.ScriptPostfix);
    }
    
    public static IScriptRunner CreateCreateBranchScriptRunner()
    {
        const string scriptName = "CreateBranch";
        return new ScriptRunner(Common.ScriptsDirPath + scriptName + Common.ScriptPostfix);
    }
    
    public static IScriptRunner CreateCommitChangesScriptRunner()
    {
        const string scriptName = "CommitChanges";
        return new ScriptRunner(Common.ScriptsDirPath + scriptName + Common.ScriptPostfix);
    }
}