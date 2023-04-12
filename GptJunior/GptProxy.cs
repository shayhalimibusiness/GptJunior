using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;

namespace GptJunior;

public interface IGptProxy
{
    Task<string> GetResponse(string massage);
    void AddToBaseRequest(string addition);
}

public class GptProxy : IGptProxy
{
    private OpenAIClient _api;
    
    public GptProxy(List<ChatPrompt> chatPrompts)
    {
        _api = new OpenAIClient("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");
        var chatRequest = new ChatRequest(chatPrompts);
        _api.ChatEndpoint.GetCompletionAsync(chatRequest).Wait();
    }

    public async Task<string> GetResponse(string massage)
    {
        var chatPrompts = new List<ChatPrompt>
        {
            new("user", massage),
        };
        var chatRequest = new ChatRequest(chatPrompts);
        var result = await _api.ChatEndpoint.GetCompletionAsync(chatRequest);
        return result.FirstChoice;
    }

    public void AddToBaseRequest(string addition)
    {
        throw new NotImplementedException();
    }
}

public class BaseReqGptProxy : IGptProxy
{
    private IFileEditor Logger;
    private string BaseReq { get; set; }
    private OpenAIClient AiClient { get; set; }

    public BaseReqGptProxy(string baseReq)
    {
        Logger = FileEditorsFactory.CreateRunLogEditor();
        BaseReq = baseReq;
        AiClient = new OpenAIClient("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");
    }

    public virtual async Task<string> GetResponse(string massage)
    {
        var chatPrompts = new List<ChatPrompt>
        {
            new("user", BaseReq + massage),
        };
        var chatRequest = new ChatRequest(chatPrompts, Model.GPT3_5_Turbo, 0.01);
        var result = await AiClient.ChatEndpoint.GetCompletionAsync(chatRequest);
        var response = (string)result.FirstChoice;
        Logger.AppendText("User:\n" + massage + "\n\nGPT:\n" + response + "\n----------------------------\n");
        return response;
    }

    public void AddToBaseRequest(string addition)
    {
        BaseReq += "\n" + addition;
    }
}

public class LoggingGptProxy : BaseReqGptProxy
{
    private ILogger Logger { get; set; }
    
    public LoggingGptProxy(string baseReq, ILogger logger) : base(baseReq)
    {
        Logger = logger;
    }

    public override async Task<string> GetResponse(string message) 
    {
        var response = await base.GetResponse(message);
        
        Logger.Log(message, ELogType.ConversationUser);
        Logger.Log(response, ELogType.ConversationBot);
        
        return response;
    }
}

public static class GptBotsFactory
{
    #region General

    private const string EndOfIntroduction = "\n------\n";

    public const string EndOfSection = "\n******\n";

    private const string EndOfRequest = "\n######\n";

    private const string Introduction =
        "This is not the request itself but instructions on what requests to aspect, and how to answer requests.\n" +
        "Between Instructions' different sections there will be this line:" + EndOfSection +
        "In the end of all of the instructions there will be this line:" + EndOfIntroduction;

    #endregion

    #region Coder

    private const string CoderRequestFormat =
        "Request Format: A description of a function.\n" +
        EndOfSection;

    private const string CoderAnswerFormat =
        "Answer Format: A function in C# that runs from Program.cs, " +
        "so it shouldn't have an accessibility token like 'public'.\n" +
        "The answer must be only the code!\n" +
        "No human text!" +
        EndOfSection;

    #endregion
    
    #region Fixer

    private const string FixerRequestFormat = 
        "The User will supply you with sections seperated by lines: " + EndOfSection +
        "In the end of the request there will be a different line: " + EndOfRequest +
        "First Section: The content of Program.cs a file inside PlayGround c# console project.\n" +
        "This is the only file with actual code and it is the entry point of the project.\n" +
        "Second Section: The content of the build log from trying to build PlayGround project.\n" +
        "If you see, multiple " + EndOfRequest + 
        "It means that it is the history of previous requests and your fix attempts.\n" +
        "Learn from it!!!" +
        EndOfSection;

    private const string FixerAnswerFormat = 
        "Your answer will be Program.cs in its fixed version.\n" +
        "Don't supply anything except for the code.\n" +
        "If you think that according to the feedback you got the code works fine, send the word: PASS\n" +
        "Exactly in this syntax.\n" +
        EndOfSection;
    
    private const string GoodBuildExample = 
        "This is an example of a good build (so you should return PASS):" +
        "MSBuild version 17.5.0+6f08c67f3 for .NET\n  " +
        "Determining projects to restore...\n  " +
        "All projects are up-to-date for restore.";

    private const string BadBuildExample = 
        "MSBuild version 17.5.0+6f08c67f3 for .NET\n" +
        "Determining projects to restore...\n  " +
                                           "All projects are up-to-date for restore.\n" +
                                           "C:\\Users\\shay.halimi\\Desktop\\DlHellGpt\\DlHellGpt\\PlayGround\\Program.cs(40,1): " +
                                           "error CS0106: The modifier 'public' is not valid for this item [C:\\Users\\shay.halimi\\Desktop\\DlHellGpt\\DlHellGpt\\PlayGround\\PlayGround.csproj]\n\nBuild FAILED.\n\nC:\\Users\\shay.halimi\\Desktop\\DlHellGpt\\DlHellGpt\\PlayGround\\Program.cs(40,1): error CS0106: The modifier 'public' is not valid for this item [C:\\Users\\shay.halimi\\Desktop\\DlHellGpt\\DlHellGpt\\PlayGround\\PlayGround.csproj]\n    0 Warning(s)\n    1 Error(s)\n\nTime Elapsed 00:00:00.95";

    #endregion

    public static IGptProxy GetScriptWriterBot()
    {
        const string baseRequest =
            "Those are instruction of how you answer. The request itself will come after a line like this: ------\n" +
            "You will get a description of a desired shell script.\n" +
            "Your purpose is to provide this script.\n" +
            "Your answer should be composed of 2 parts:\n" +
            "1. The name and only the name of the script without the .bat postfix. This will be the first line.\n" +
            "2. The script code. Only this, with no other additions.\n" +
            "After this, you don't give more output. You finish.\n" +
            "Example: \n" +
            "User: Give me a script that print out Hello World!\n" +
            "You: HelloWorldPrinter\n" +
            "@echo off\n" +
            "echo Hello World!\n" +
            "------\n";
        return new BaseReqGptProxy(baseRequest);
    }
    
    public static IGptProxy GetScripTesterBot()
    {
        const string baseRequest =
            "Those are instruction of how you answer. The request itself will come after a line like this: ------\n" +
            "The request, you will get, composed of 4 parts:\n" +
            "1. Instruction for a script.\n" +
            "2. The script name.\n" +
            "3. The script itself\n" +
            "4. The output of a run of this script.\n" +
            "Between each part there will be a line of: ******\n" +
            "Your purpose is to determine if the script works.\n" +
            "Your answer should be one word only!!! True or False.\n" +
            "This should reflect if the script works (True) or fails (False).\n" +
            "If you are not sure, take a guess." +
            "------";
        return new BaseReqGptProxy(baseRequest);
    }

    public static IGptProxy GetFlowBot()
    {
        const string baseRequest =
            "This is not the request itself but instructions on what requests to aspect, and how to answer requests.\n" +
            "In the end of those instructions there will be a line:\n------\n\n" +
            "The request is going to be in these exact format:\n" +
            "1. First line, a signature of a function.\n" +
            "2. \n******\n" +
            "3. The request to a junior programmer to create the function in the signature.\n\n" +
            "With the signature and the request to the junior, deduct what the function should do,\n" +
            "and build a flow in C# code, that run the function and, if possible, print the result.\n" +
            "Answer only with the flow! No introductions! No comments after the code! only the code itself!\n" +
            "And no implementation of the Function!\n" +
            "Example:\n" +
            "User: int AddTwo(int n) {\n" +
            "A function that returns a number plus 2.\n" +
            "You: int x = AddTwo(3);\n" +
            @"Console.WriteLine($""This should be five : {x}"");\n" +
            "~End of Example\n" +
            "AddTwo is the name of the function from the function signature. It will change depending on the signature provided\n" +
            "------\n";
        var gptLogger = LoggersFactory.CreateConversationLogger("FlowGpt");
        
        return new LoggingGptProxy(baseRequest, gptLogger);
    }

    public static IGptProxy GetCodeFixer()
    {
        const string baseRequest =
            Introduction +
            FixerRequestFormat +
            FixerAnswerFormat +
            GoodBuildExample +
            EndOfIntroduction;
        var gptLogger = LoggersFactory.CreateConversationLogger("FixerGpt");

        return new LoggingGptProxy(baseRequest, gptLogger);
    }

    public static IGptProxy CreateCodeBot()
    {
        const string baseRequest =
            Introduction +
            CoderRequestFormat +
            CoderAnswerFormat +
            EndOfIntroduction;
        var gptLogger = LoggersFactory.CreateConversationLogger("CodeGpt");

        return new LoggingGptProxy(baseRequest, gptLogger);
    }
}