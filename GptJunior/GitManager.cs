namespace GptJunior;

public interface IGitManager
{
    void CreateBranch(string name);
    string CommitChanges(string commitMassage);
}

public class GitManager : IGitManager
{
    private string? _branchName;
    private IScriptRunner _createBranchScript;
    private IScriptRunner _commitChangesScript;
    private IFileEditor _createBranchEditor;
    private IFileEditor _commitChangesEditor;
    
    public GitManager(
        IScriptRunner createBranchScript, 
        IScriptRunner commitChangesScript, 
        IFileEditor createBranchEditor, 
        IFileEditor commitChangesEditor)
    {
        _createBranchScript = createBranchScript;
        _commitChangesScript = commitChangesScript;
        _createBranchEditor = createBranchEditor;
        _commitChangesEditor = commitChangesEditor;
    }

    public void CreateBranch(string name)
    {
        _branchName = name;
        var script = 
            "@echo off\n" +
            $"git checkout -b {name}";
        _createBranchEditor.WriteFile(script);
        _createBranchScript.Run();
    }

    public string CommitChanges(string commitMassage)
    {
        if (_branchName == null)
        {
            return "";
        }
        
        var script =
            "@echo off\n" +
            "git add --all\n" +
            $"git commit -m \"{commitMassage}\"\n" +
            "git checkout master";
        _commitChangesEditor.WriteFile(script);
        _commitChangesScript.Run();
        return _branchName;
    }
}

public static class GitManagersFactory
{
    public static IGitManager CreateGitManager()
    {
        var createBranchScriptRunner = ScriptRunnerFactory.CreateCreateBranchScriptRunner();
        var commitChangesScriptRunner = ScriptRunnerFactory.CreateCommitChangesScriptRunner();
        var createBranchEditor = FileEditorsFactory.CreateCreateBranchEditor();
        var commitChangesEditor = FileEditorsFactory.CreateCommitChangesEditor();

        var gitManager = new GitManager(
            createBranchScriptRunner,
            commitChangesScriptRunner,
            createBranchEditor,
            commitChangesEditor);

        return gitManager;
    }
}