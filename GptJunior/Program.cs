// See https://aka.ms/new-console-template for more information

using GptJunior;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;


await GetResponse("Hi");

// try
// {
//     var gptProxy = GptProxiesFactory.CreateCodeBot();
//     Console.WriteLine(gptProxy.GetResponse("function that returns a number plus 2."));
// }
// catch (Exception e)
// {
//     Console.WriteLine(e);
// }
//
// Console.WriteLine("Hello, World!");


async Task<string> GetResponse(string message) 
{
    OpenAIClient client = new OpenAIClient("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");
    var chatPrompts = new List<ChatPrompt>
    {
        new("user", "Hi"),
    };
    var chatRequest = new ChatRequest(chatPrompts, Model.GPT3_5_Turbo, 0.01);
    var result = await client.ChatEndpoint.GetCompletionAsync(chatRequest);
    Console.WriteLine(result);
    return result.FirstChoice;
}

// var gptJunior = GptJuniorsFactory.CreateGptJunior();
// gptJunior.Create("function that return 'Hodaya' string");