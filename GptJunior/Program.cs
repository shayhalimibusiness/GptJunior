// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using GptJunior;
using GptJunior.Modules;

var gptTester = GptProxiesFactory.CreateGptTester();
var response =
    await gptTester.GetResponse("Interface that can takes number and bring back the sum of all the numbers it got so far.");
DevAnswer jsonAns = JsonSerializer.Deserialize<DevAnswer>(response);
Console.WriteLine(response);

// var res = await Tester.GetTokenLimit(800);
// Console.WriteLine(res);
// var str = Tester.GetRandomNumbersString(800);
// Console.WriteLine(str);

// var gitManager = GitManagersFactory.CreateGitManager();
// gitManager.CreateBranch("testingIt");
// gitManager.CommitChanges("Initial Commit.");

// var projectManager = ProjectManagersFactory.CreateProjectManager();
// var gptDeveloper = GptDevelopersFactory.CreateGptDeveloper();
//
// var gptJunior = new GptJunior.GptJunior(projectManager, gptDeveloper);

// var gptJunior = GptJuniorsFactory.CreateGptJunior();
// await gptJunior.Create("a function that Create a graph from nothing. The graph has 3 nodes and 2 edges.");

Console.ReadLine();

