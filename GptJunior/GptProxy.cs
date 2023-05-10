using OpenAI.Chat;
using OpenAI.Models;
using OpenAIClient = OpenAI.OpenAIClient;
using OpenAILibClient = OpenAILib.OpenAIClient;

namespace GptJunior;

public interface IGptProxy
{
    Task<string> GetResponse(string massage);
    void AddToBaseRequest(string addition);
}

public class GptProxy : IGptProxy
{
    private readonly OpenAIClient _api;
    
    public GptProxy(List<ChatPrompt> chatPrompts)
    {
        // ReSharper disable StringLiteralTypo
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
    private string BaseReq { get; set; }
    private OpenAIClient AiClient { get; set; }

    public BaseReqGptProxy(string baseReq)
    {
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

public static class GptProxiesFactory
{
    #region General

    private const string EndOfIntroduction = "\n------\n";
    private const string EndOfIntroductionSection = "\n******\n";
    private const string EndOfRequest = "\n######\n";
    private const string EndOfConversationSection = "\n>>>>>\n";

    private const string OldIntroduction =
        "This is not the request itself but instructions on what requests to aspect, and how to answer requests.\n" +
        "Between Instructions' different sections there will be this line:" + EndOfIntroductionSection +
        "In the end of all of the instructions there will be this line:" + EndOfIntroduction;

    #endregion

    #region Coder

    private const string CoderRequestFormat =
        "Request Format: A description of a function.\n" +
        EndOfIntroductionSection;

    private const string CoderAnswerFormat =
        "Answer Format: A function in C# that runs from Program.cs, " +
        "so it shouldn't have an accessibility token like 'public'.\n" +
        "The answer must be only the code!\n" +
        "No human text!" +
        EndOfIntroductionSection;

    #endregion
    
    #region Fixer

    private const string FixerRequestFormat = 
        "The User will supply you with sections seperated by lines: " + EndOfIntroductionSection +
        "In the end of the request there will be a different line: " + EndOfRequest +
        "First Section: The content of Program.cs a file inside PlayGround c# console project.\n" +
        "This is the only file with actual code and it is the entry point of the project.\n" +
        "Second Section: The content of the build log from trying to build PlayGround project.\n" +
        "If you see, multiple " + EndOfRequest + 
        "It means that it is the history of previous requests and your fix attempts.\n" +
        "Learn from it!!!" +
        EndOfIntroductionSection;

    private const string FixerAnswerFormat = 
        "Your answer will be Program.cs in its fixed version.\n" +
        "Don't supply anything except for the code.\n" +
        "If you think that according to the feedback you got the code works fine, send the word: PASS\n" +
        "Exactly in this syntax.\n" +
        EndOfIntroductionSection;
    
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

    #region FunctionDeveloper

    private const string FunctionDeveloperRequestFormat = 
        "Expected request format:\n" +
        "A description of a function." +
        EndOfIntroductionSection;
    private const string FunctionDeveloperAnswerFormat = 
        "Expected answer format:\n" +
        " - Function name.\n" +
        " - This line:" + EndOfConversationSection + 
        " - Function implementation.\n" +
        " - This line:" + EndOfConversationSection +
        " - Function flow: A use of this function with the appropriate arguments. Print the result.\n" +
        " - This line:" + EndOfConversationSection +
        " - The expected return result from the flow." +
        " - This line:" + EndOfConversationSection +
        EndOfIntroductionSection;
    private const string FunctionDeveloperRequestExample = 
        "This is an example to a request that I could give you:\n" +
        "function that returns a number plus 2" +
        EndOfIntroductionSection;
    private const string FunctionDeveloperAnswerExample = 
        "This is the answer I expect from you based on the request example:\n" +
        "AddTwo" +
        EndOfConversationSection +
        "public int AddTwo(int num)\n{\n   return num + 2;\n}" +
        EndOfConversationSection +
        "int result = AddTwo(5);\nConsole.WriteLine(result);" +
        EndOfConversationSection +
        "7" +
        EndOfConversationSection +
        EndOfIntroductionSection;
    private const string FunctionDeveloperNotes = 
        "Of course you don't give that answer on every request. this is only an example on what you should have answered" +
        "if I gave you a request like in the example." +
        "The language is C#.\n" +
        "Remember to give only! only! the content of the expected answer and nothing else!" +
        EndOfIntroductionSection;

    #endregion

    #region FunctionFixer

    private const string FunctionFixerRequestFormat = 
        "Expected request format:\n" +
        " - Function name.\n" +
        " - This line:" + EndOfConversationSection + 
        " - Function description.\n" +
        " - This line:" + EndOfConversationSection +
        " - A class file content with the function implementation.\n" +
        " - This line:" + EndOfConversationSection +
        " - A Program.cs file content with this function use.\n" +
        " - This line:" + EndOfConversationSection +
        " - build_log.txt of building the project with:\n" +
        "   dotnet build -o ../output PlayGround.csproj > ../output/build_log.txt 2>&1\n" +
        "   The only cs files in the project are Program and the class file." +
        " - This line:" + EndOfConversationSection +
        " - run_log.txt of running the project with:\n" +
        @"   output\PlayGround.exe > ./output/run_log.txt 2>&1\n" +
        " - This line:" + EndOfConversationSection +
        " - Expected result of the run_log:\n" +
        " - This line:" + EndOfConversationSection +
        EndOfIntroductionSection;
    private const string FunctionFixerNotes = 
        "Base on this information you will have to figure out if the project was " +
        "build correctly and the function behaved as expected.\n" +
        "If not figure out what is the problem and how to fix it.\n" +
        EndOfIntroductionSection;
    private const string FunctionFixerAnswerFormat = 
        "Expected answer format:\n" +
        " - Does the project works? if so, answer with: PASS!, otherwise: FAILED!.\n" +
        "   If you answered PASS! finish your answer here, if answered FAILED!, continue:\n" +
        " - This line:" + EndOfConversationSection + 
        " - New Program.cs content. This should still include the flow, but if there was a bug there, fix it.\n" +
        " - This line:" + EndOfConversationSection +
        " - New class file content. This should still include the function, but if there was a bug there" +
        "or a implementation issue, fix it (the function still needs to match its description).\n" +
        " - This line:" + EndOfConversationSection +
        EndOfIntroductionSection;
    // Todo: edit
    private const string FunctionFixerRequestExample = 
        "This is an example to a request that I could give you:\n" +
        "CreateGraph\n*******************************************\nClass file:\nnamespace PlayGround\n{\n    public class CreateGraphClass\n    {\n        // Add your class members  here\n        \n        // Add your class  methods here\n        public Graph CreateGraph()\n        {\n           Graph graph = new Graph();\n           Node node1 = new Node(1);\n           Node node2 = new Node(2);\n           Node node3 = new Node(3);\n           graph.AddNode(node1);\n           graph.AddNode(node2);\n           graph.AddNode(node3);\n           graph.AddEdge(node1, node2);\n           graph.AddEdge(node2, node3);\n           return graph;\n        }\n        \n        public void Test()\n        {\n           Graph graph = CreateGraph();\n           Console.WriteLine(graph.ToString());\n        }\n    }\n}\n\n*******************************************\nProgram.cs:\nusing PlayGround;\n\n\n// Add your flow  here\nvar test = new CreateGraphClass();\ntest.Test();\n\n*******************************************\nBuild:\nMSBuild version 17.5.1+f6fdcf537 for .NET\n  Determining projects to restore...\n  All projects are up-to-date for restore.\nC:\\Users\\shay.halimi\\Desktop\\PrivateWorkPlace\\PlayGround\\PlayGround\\CreateGraphClass.cs(8,16): error CS0246: The type or namespace name 'Graph' could not be found (are you missing a using directive or an assembly reference?) [C:\\Users\\shay.halimi\\Desktop\\PrivateWorkPlace\\PlayGround\\PlayGround\\PlayGround.csproj]\n\nBuild FAILED.\n\nC:\\Users\\shay.halimi\\Desktop\\PrivateWorkPlace\\PlayGround\\PlayGround\\CreateGraphClass.cs(8,16): error CS0246: The type or namespace name 'Graph' could not be found (are you missing a using directive or an assembly reference?) [C:\\Users\\shay.halimi\\Desktop\\PrivateWorkPlace\\PlayGround\\PlayGround\\PlayGround.csproj]\n    0 Warning(s)\n    1 Error(s)\n\nTime Elapsed 00:00:01.35\n\n*******************************************\nRun:\n5 2\n\n*******************************************\nExpected Run Log:\n1 -> 2\n*******************************************\n\n---------------------------------------\n" +
        EndOfIntroductionSection;

    private const string FunctionFixerAnswerExample =
        "This is an example to how you should answer the previous request:\n" +
        "FAILED!\n>>>>>\nusing PlayGround;\n\n\n// Add your flow  here\nvar test = new CreateGraphClass();\ntest.Test();\n\n>>>>>\nnamespace PlayGround\n{\n    public class Graph\n    {\n        private List<Node> nodes = new List<Node>();\n        private Dictionary<Node, List<Node>> edges = new Dictionary<Node, List<Node>>();\n\n        public void AddNode(Node node)\n        {\n            nodes.Add(node);\n        }\n\n        public void AddEdge(Node node1, Node node2)\n        {\n            if (!edges.ContainsKey(node1))\n            {\n                edges[node1] = new List<Node>();\n            }\n            edges[node1].Add(node2);\n        }\n\n        public override string ToString()\n        {\n            string result = \"\";\n            foreach (var node in nodes)\n            {\n                result += node.Value + \" -> \";\n                if (edges.ContainsKey(node))\n                {\n                    foreach (var neighbor in edges[node])\n                    {\n                        result += neighbor.Value + \" \";\n                    }\n                }\n                result += \"\\n\";\n            }\n            return result;\n        }\n    }\n\n    public class Node\n    {\n        public int Value { get; set; }\n\n        public Node(int value)\n        {\n            Value = value;\n        }\n    }\n\n    public class CreateGraphClass\n    {\n        // Add your class members  here\n\n        // Add your class  methods here\n        public Graph CreateGraph()\n        {\n            Graph graph = new Graph();\n            Node node1 = new Node(1);\n            Node node2 = new Node(2);\n            Node node3 = new Node(3);\n            graph.AddNode(node1);\n            graph.AddNode(node2);\n            graph.AddNode(node3);\n            graph.AddEdge(node1, node2);\n            graph.AddEdge(node2, node3);\n            return graph;\n        }\n\n        public void Test()\n        {\n            Graph graph = CreateGraph();\n            Console.WriteLine(graph.ToString());\n        }\n    }\n}\n>>>>>";

    #endregion

    #region InterfaceDeveloper

    private const string UserRequestToken = "<UserRequest>";
    private const string DescriptionToken = "<Description>";
    private const string ListToken = "<List>";
    

    private const string Introduction =
        $"This is an explanation on how to reply to a request. \nThe request will begin after the token: {UserRequestToken}.";
    
    private const string InterfaceDeveloperInstructions = 
        "The request will contain a description of an interface.\nYour answer should be the interface, an implementation of the interface, \na factory class with a function that creates an instance from this implementation,\nUnit tests that test the implementation and the necessary code to support it.\nThe code is in C#.\nenums to be familiar with:\npublic enum EDesignPattern\n{ // each value represent the design pattern code the file contains.\n    Interface, \n    Implementation,\n    Factory,\n    UnitTests\n}\nThe format of your answer will be json:\n{\n\t\"\"Name\"\": <Name for the interface, without the I C# prefix>,\n\t\"\"Inter\"\": <The content of the .cs file that contains the interface.>,\n\t\"\"Implementation\"\": [<List of files for the interface, implementation factory and code, and their content>\n\t\t{\n\t\t\t\"\"FileName\"\": \"\"fileNum1\"\",\n\t\t\t\"\"DesignPattern\"\": <Enum EDesignPattern Type>\n\t\t\t\"\"FileContent\"\": <the code in the file. Add indetations and \\n according to rider formatting rules for C# (Rule symbol:***)>\n\t\t},\n\t\t{\n\t\t\t\"\"FileName\"\": \"\"fileNum2\"\",\n\t\t\t\"\"DesignPattern\"\": <Enum EDesignPattern Type>\n\t\t\t\"\"FileContent\"\": <***>\n\t\t},...\n\t\t],\n\t\"\"Tests\"\": [<List of files for the unit tests and their content:>\n\t\t{\n\t\t\t\"\"FileName\"\": \"\"fileNum1\"\",\n\t\t\t\"\"DesignPattern\"\": 3,\n\t\t\t\"\"FileContent\"\": <***>\n\t\t},\n\t\t{\n\t\t\t\"\"FileName\"\": \"\"fileNum2\"\",\n\t\t\t\"\"DesignPattern\"\": 3,\n\t\t\t\"\"FileContent\"\": <***>\n\t\t},...\n\t\t],\n}\n\n";

    private const string InterfaceFixerInstructions = 
        "The request is feedback on a new code (the code in C#).\nThe request will be in json format and is equal to this class:\npublic class FixRequest\n{\n    public string? Description { get; set; } // Description of what the code supposed to do.\n    public string? Build { get; set; } // The build log of the code.\n    public string? Run { get; set; } // The Run log of the code, including Unit Tests for it. \n    public List<FileDto>? Files { get; set; } // A list of files with the code inside. \n}\npublic class FileDto\n{\n    public string? FileName { get; set; }\n    public EDesignPattern? DesignPattern { get; set; }\n    public string? FileContent { get; set; }\n}\npublic enum EDesignPattern\n{\n    Interface,\n    Implementation,\n    Factory,\n    UnitTests\n}\n\nYour answer should be a list of files that needs correction:\nThe format of your answer will be json:\n{\n\t\"\"Files\"\": [\n\t\t<List of files that needs correction and the correction itself. The list will be empty if the code seems to work fine and the is no need to edit any file.>\n\t\t{\n\t\t\t\"\"FileName\"\": \"\"fileNum1\"\",\n\t\t\t\"\"DesignPattern\"\": <Same as it was>\n\t\t\t\"\"FileContent\"\": <the code in the file. Add indetations and \\n according to rider formatting rules for C# (Rule symbol:***)>\n\t\t},\n\t\t{\n\t\t\t\"\"FileName\"\": \"\"fileNum2\"\",\n\t\t\t\"\"DesignPattern\"\": <Same as it was>\n\t\t\t\"\"FileContent\"\": <***>\n\t\t},\n\t\t...\n\t]\n}";
    
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
            OldIntroduction +
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
            OldIntroduction +
            CoderRequestFormat +
            CoderAnswerFormat +
            EndOfIntroduction;
        var gptLogger = LoggersFactory.CreateConversationLogger("CodeGpt");

        return new LoggingGptProxy(baseRequest, gptLogger);
    }

    public static IGptProxy CreateGptFunctionDeveloper()
    {
        const string baseRequest = OldIntroduction +
                                   FunctionDeveloperRequestFormat +
                                   FunctionDeveloperAnswerFormat +
                                   FunctionDeveloperRequestExample +
                                   FunctionDeveloperAnswerExample +
                                   FunctionDeveloperNotes +
                                   EndOfIntroduction;
        
        var logger = LoggersFactory.CreateConversationLogger("GptFunctionDeveloper");

        var gptFunctionDeveloper = new LoggingGptProxy(baseRequest, logger);

        return gptFunctionDeveloper;
    }
    
    public static IGptProxy CreateGptFunctionFixer()
    {
        const string baseRequest = OldIntroduction +
                                   FunctionFixerRequestFormat +
                                   FunctionFixerNotes +
                                   FunctionFixerAnswerFormat +
                                   FunctionFixerRequestExample +
                                   FunctionFixerAnswerExample +
                                   EndOfIntroduction;
        
        var logger = LoggersFactory.CreateConversationLogger("GptFunctionFixer");

        var gptFunctionDeveloper = new LoggingGptProxy(baseRequest, logger);

        return gptFunctionDeveloper;
    }

    public static IGptProxy CreateInterfaceDeveloper()
    {
        const string baseRequest =
            Introduction +
            InterfaceDeveloperInstructions +
            UserRequestToken;
        
        var logger = LoggersFactory.CreateConversationLogger("InterfaceDeveloper");

        var gptFunctionDeveloper = new LoggingGptProxy(baseRequest, logger);

        return gptFunctionDeveloper;
    }
    
    public static IGptProxy CreateInterfaceFixer()
    {
        const string baseRequest =
            Introduction +
            InterfaceFixerInstructions +
            UserRequestToken;
        
        var logger = LoggersFactory.CreateConversationLogger("InterfaceFixer");

        var gptFunctionDeveloper = new LoggingGptProxy(baseRequest, logger);

        return gptFunctionDeveloper;
    }
}