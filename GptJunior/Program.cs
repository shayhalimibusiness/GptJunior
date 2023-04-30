// See https://aka.ms/new-console-template for more information

using GptJunior;

// var gitManager = GitManagersFactory.CreateGitManager();
// gitManager.CreateBranch("testingIt");
// gitManager.CommitChanges("Initial Commit.");

// var projectManager = ProjectManagersFactory.CreateProjectManager();
// var gptDeveloper = GptDevelopersFactory.CreateGptDeveloper();
//
// var gptJunior = new GptJunior.GptJunior(projectManager, gptDeveloper);

var gptJunior = GptJuniorsFactory.CreateGptJunior();
await gptJunior.Create("a function that Create a graph from nothing. The graph has 3 nodes and 2 edges.");

Console.ReadLine();

