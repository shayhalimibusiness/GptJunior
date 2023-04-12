namespace GptJunior;

public interface ILogger
{
    void Log(string massage, ELogType logType = ELogType.Normal);
    void CleanLog();
}

public class Logger : ILogger
{
    private const string LogLine = "\n---------------------------------------\n";

    private readonly string _logName;
    
    private readonly IFileEditor _logEditor;
    
    public Logger(IFileEditor log, string logName)
    {
        _logEditor = log;
        _logName = logName;
    }

    public void Log(string massage, ELogType logType = ELogType.Normal)
    {
        
        _logEditor.AppendText(LogLine);
        _logEditor.AppendText("[" + _logName + "]");
        _logEditor.AppendText(LogLine);
        _logEditor.AppendText(massage);
        _logEditor.AppendText(LogLine);
    }

    public void CleanLog()
    {
        _logEditor.CleanFile();
    }
}

public class ConversationLogger : ILogger
{
    private const string LogLine = "\n---------------------------------------\n";
    private const string UserName = "UserName";

    private readonly string _logName;
    private readonly string _adversarialName;
    
    private readonly IFileEditor _logEditorEditor;
    
    public ConversationLogger(IFileEditor logEditor, string logName, string adversarialName)
    {
        _logEditorEditor = logEditor;
        _logName = logName;
        _adversarialName = adversarialName;
    }

    public void Log(string massage, ELogType logType = ELogType.ConversationUser)
    {
        var conversationSide = logType == ELogType.ConversationUser ? UserName : _adversarialName;
        
        _logEditorEditor.AppendText(LogLine);
        _logEditorEditor.AppendText("[" + _logName + "] [" + conversationSide + "]");
        _logEditorEditor.AppendText(LogLine);
        _logEditorEditor.AppendText(massage);
        _logEditorEditor.AppendText(LogLine);
    }

    public void CleanLog()
    {
        _logEditorEditor.CleanFile();
    }
}

public static class LoggersFactory
{
    public static ILogger CreateConversationLogger(string adversarialName)
    {
        var logEditor = FileEditorsFactory.CreateConversationLogEditor();
        const string logName = "Conversation";

        var conversationLogger = new ConversationLogger(logEditor, logName, adversarialName);

        return conversationLogger;
    }
}