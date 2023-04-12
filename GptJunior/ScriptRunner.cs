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
    private const string ScriptsDir = @"C:\Users\shay.halimi\Desktop\GptJunior\PlayGround\PlayGround\scripts\";
    private const string ScriptPostfix = ".bat";
    
    public static IScriptRunner CreateRunProjectScriptRunner()
    {
        const string scriptName = "RunProject";
        return new ScriptRunner(ScriptsDir + scriptName + ScriptPostfix);
    }
}