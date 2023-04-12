namespace GptJunior;

public interface IFeedbackViewer
{
    string GetBuildLog();
    string GetRunLog();
}

public class FeedbackViewer : IFeedbackViewer
{
    private readonly IFileEditor _buildLogEditor;
    private readonly IFileEditor _runLogEditor;
    
    public FeedbackViewer(IFileEditor buildLogEditor, IFileEditor runLogEditor)
    {
        _buildLogEditor = buildLogEditor;
        _runLogEditor = runLogEditor;
    }

    public string GetBuildLog()
    {
        return _buildLogEditor.ReadFile();
    }

    public string GetRunLog()
    {
        return _runLogEditor.ReadFile();
    }
}

public static class FeedbackViewersFactory
{
    public static IFeedbackViewer CreateProjectFeedbackViewer()
    {
        var buildLogEditor = FileEditorsFactory.CreateBuildLogEditor();
        var runLogEditor = FileEditorsFactory.CreateRunLogEditor();

        var feedbackViewer = new FeedbackViewer(buildLogEditor, runLogEditor);

        return feedbackViewer;
    }
}